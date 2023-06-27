namespace Chess
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;

    public class GraphicsHandler : MonoBehaviour
    {
        public GameObject squarePrefab;
        public GameObject coordTextPrefab;
        public Color darkSquare;
        public Color lightSquare;
        public Color highlightedDarkSquare;
        public Color highlightedLightSquare;
        public Color moveSquareColor;
        public Sprite[] piecesSprites;
        //public static string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        float squareSize = 1f;
        Queue<GameObject> availablePieces;
        GameObject[] squares;

        #region  Singleton
        public static GraphicsHandler handler = null;

        private void Awake()
        {
            if (handler != null)
                Debug.LogError("More than one Graphic handler in the scene!", gameObject);
            handler = this;
        }
        #endregion

        public void StartGame()
        {
            InitializeQueue();
            ReadFEN();
            CreateBoard();
            CreatePieces();
        }

        void InitializeQueue()
        {
            availablePieces = new Queue<GameObject>();

            for (int i = 0; i < 32; i++)
            {
                GameObject piece = new GameObject("Piece");
                piece.tag = "Piece";
                piece.layer = LayerMask.NameToLayer("Pieces");
                piece.transform.parent = transform.GetChild(0);
                piece.AddComponent<ChessPiece>();
                piece.AddComponent<SpriteRenderer>();
                piece.GetComponent<SpriteRenderer>().sortingOrder = 1;
                piece.AddComponent<BoxCollider2D>().enabled = false;
                piece.SetActive(false);

                availablePieces.Enqueue(piece);
            }
        }

        void ReadFEN()
        {
            FENReading fen = Essentials.ReadFEN(Board.gameFEN);
            Board.pieces = fen.pieces;
            Board.turnToMove = fen.turn;
            Board.enPassantSquare = fen.enPassantTS;
            Board.whiteCastlingRights = fen.whiteCastlingRights;
            Board.blackCastlingRights = fen.blackCastlingRights;

            Board.opponentThreatMap = MovesGenerator.GenerateThreatMap();

            GameHandler.handler.InitUI();
        }

        void CreateBoard()
        {
            squares = new GameObject[64];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameObject square = Instantiate(squarePrefab, new Vector3((j - 3.5f) * squareSize, (i - 3.5f) * squareSize, 0), Quaternion.identity, transform.GetChild(1));
                    square.GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? darkSquare : lightSquare;
                    square.GetComponent<SpriteRenderer>().sortingOrder = 0;
                    square.transform.localScale = Vector3.one * squareSize;
                    square.GetComponent<ChessSquare>().pos = new int[2] { j, i };

                    squares[i * 8 + j] = square;
                }
            }

            GameObject tableTopLight = Instantiate(squarePrefab, Vector3.zero, Quaternion.identity, transform.GetChild(1));
            tableTopLight.GetComponent<SpriteRenderer>().color = lightSquare;
            tableTopLight.GetComponent<SpriteRenderer>().sortingOrder = -1;
            tableTopLight.transform.localScale = Vector3.one * 8.2f * (float)squareSize;
            Destroy(tableTopLight.GetComponent<ChessSquare>());
            Destroy(tableTopLight.GetComponent<BoxCollider2D>());

            GameObject tableTopDark = Instantiate(squarePrefab, Vector3.zero, Quaternion.identity, transform.GetChild(1));
            tableTopDark.GetComponent<SpriteRenderer>().color = darkSquare;
            tableTopDark.GetComponent<SpriteRenderer>().sortingOrder = -2;
            tableTopDark.transform.localScale = Vector3.one * 9.6f * (float)squareSize;
            Destroy(tableTopDark.GetComponent<ChessSquare>());
            Destroy(tableTopDark.GetComponent<BoxCollider2D>());

            for (int i = 0; i < 8; i++)
            {
                GameObject text = Instantiate(coordTextPrefab, new Vector3((i - 3.5f) * squareSize, -4.45f * squareSize, 0), Quaternion.identity, transform.GetChild(2));
                text.GetComponent<TextMeshProUGUI>().text = ((char)(i + 65)).ToString().ToLower();
            }
            for (int i = 0; i < 8; i++)
            {
                GameObject text = Instantiate(coordTextPrefab, new Vector3(-4.45f * squareSize, (i - 3.5f) * squareSize, .1f), Quaternion.identity, transform.GetChild(2));
                text.GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            }
        }

        void CreatePieces()
        {
            for (int i = 0; i < Board.pieces.Count; i++)
            {
                int[] pos = Board.pieces[i].position;

                int pieceType = Board.pieces[i].type & 7;
                int pieceColor = Board.pieces[i].type & 24;
                int index = pieceType + 6 * (pieceColor == ChessPieceTypes.White ? 0 : 1) - 1;

                GameObject piece = availablePieces.Dequeue();
                piece.SetActive(true);
                piece.transform.position = new Vector3((pos[0] - 3.5f) * squareSize, (pos[1] - 3.5f) * squareSize, 0f);

                piece.GetComponent<SpriteRenderer>().sprite = piecesSprites[index];
                piece.GetComponent<BoxCollider2D>().enabled = true;
                piece.GetComponent<BoxCollider2D>().size = new Vector2(1, 1) * squareSize;
                piece.GetComponent<ChessPiece>().identity = Board.pieces[i];
                piece.GetComponent<ChessPiece>().identity.pieceObject = Board.pieces[i].pieceObject = piece;

                if (Board.enPassantSquare != null && Board.enPassantSquare.SequenceEqual(Board.pieces[i].position))
                    Board.enPassantPiece = Board.pieces[i];
            }
        }

        public void ResetBoard(bool resetMoved)
        {
            for (int i = 0; i < squares.Length; i++)
            {
                if (!resetMoved && squares[i].GetComponent<SpriteRenderer>().color == moveSquareColor)
                    continue;

                int[] pos = squares[i].GetComponent<ChessSquare>().pos;
                squares[i].GetComponent<SpriteRenderer>().color = ((pos[0] + pos[1]) % 2 == 0) ? darkSquare : lightSquare;
            }
        }

        public void HighlightGeneratedMoves()
        {
            int[][] moves = Board.generatedMoves.ToArray();

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                squares[moves[i][0] + moves[i][1] * 8].GetComponent<SpriteRenderer>().color = ((moves[i][0] + moves[i][1]) % 2 == 0) ? highlightedDarkSquare : highlightedLightSquare;
            }
        }

        public void HighlightMove(int[] startSquare, int[] endSquare)
        {
            squares[startSquare[0] + startSquare[1] * 8].GetComponent<SpriteRenderer>().color = moveSquareColor;
            squares[endSquare[0] + endSquare[1] * 8].GetComponent<SpriteRenderer>().color = moveSquareColor;
        }

        public void MovePiece(int[] from, int[] to) => Board.pieces.Find(x => x.position.SequenceEqual(from)).pieceObject.transform.position = new Vector3((to[0] - 3.5f) * squareSize, (to[1] - 3.5f) * squareSize, 0f);

        public GameObject UpgradePiece(Piece oldPiece, Piece newPiece, int newType)
        {
            int pieceType = newType;
            int pieceColor = Essentials.GetColor(newPiece);
            int index = pieceType + 6 * (pieceColor == ChessPieceTypes.White ? 0 : 1) - 1;
            int[] pos = newPiece.position;

            DestroyPiece(oldPiece.pieceObject);

            GameObject newPieceObject = availablePieces.Dequeue();
            newPieceObject.SetActive(true);
            newPieceObject.transform.position = new Vector3((pos[0] - 3.5f) * squareSize, (pos[1] - 3.5f) * squareSize, 0f);

            newPiece.pieceObject = newPieceObject;
            newPieceObject.GetComponent<SpriteRenderer>().sprite = piecesSprites[index];
            newPieceObject.GetComponent<BoxCollider2D>().enabled = true;
            newPieceObject.GetComponent<BoxCollider2D>().size = new Vector2(1, 1) * squareSize;
            newPieceObject.GetComponent<ChessPiece>().identity = newPiece;

            Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(newPiece.position)).pieceObject = newPieceObject;

            return newPieceObject;
        }

        public GameObject MakePiece(Piece piece)
        {
            int pieceType = Essentials.GetType(piece);
            int pieceColor = Essentials.GetColor(piece);
            int index = pieceType + 6 * (pieceColor == ChessPieceTypes.White ? 0 : 1) - 1;
            int[] pos = piece.position;

            GameObject newPieceObject = availablePieces.Dequeue();
            newPieceObject.SetActive(true);
            newPieceObject.transform.position = new Vector3((pos[0] - 3.5f) * squareSize, (pos[1] - 3.5f) * squareSize, 0f);
            piece.pieceObject = newPieceObject;

            newPieceObject.GetComponent<SpriteRenderer>().sprite = piecesSprites[index];
            newPieceObject.GetComponent<BoxCollider2D>().enabled = true;
            newPieceObject.GetComponent<BoxCollider2D>().size = new Vector2(1, 1) * squareSize;
            newPieceObject.GetComponent<ChessPiece>().identity = piece;

            Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(piece.position)).pieceObject = newPieceObject;

            return newPieceObject;
        }

        public void DestroyPiece(GameObject piece)
        {
            piece.GetComponent<ChessPiece>().identity = null;
            piece.GetComponent<SpriteRenderer>().sprite = null;
            piece.GetComponent<BoxCollider2D>().enabled = false;
            piece.SetActive(false);

            availablePieces.Enqueue(piece);
        }
    }
}