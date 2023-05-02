using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GraphicsHandler : MonoBehaviour
{
    public static GraphicsHandler handler = null;

    public gameModes gameMode;
    public GameObject squarePrefab;
    public Color darkSquare;
    public Color lightSquare;
    public Color highlightedDarkSquare;
    public Color highlightedLightSquare;
    public float squareSize = 1f;
    public GameObject[] piecesPrefabs;

    public enum gameModes 
    {
        PvP, PvB, BvB
    }
    
    const string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    GameObject[] squares;

    private void Awake()
    {
        if(handler != null)
            Debug.LogError("More than one Graphic handler in the scene!", gameObject);
        handler = this;
    }

    private void Start()
    {
        ReadFEN();
        CreateBoard();
        CreatePieces();
    }

    public void ResetBoard(){
        for (int i = 0; i < squares.Length; i++)
        {
            if (squares[i] == null)
                continue;

            int[] pos = squares[i].GetComponent<ChessSquare>().pos;
            squares[i].GetComponent<SpriteRenderer>().color = ((pos[0] + pos[1]) % 2 == 0) ? darkSquare : lightSquare;
        }
    }

    public void HighlightMove(){
        int[][] moves = Essentials.GenerateMoves(Board.selectedPiece).ToArray();

        for (int i = 0; i < moves.GetLength(0); i++)
        {
            if (squares[i] == null)
                continue;

            squares[moves[i][0] + moves[i][1] * 8].GetComponent<SpriteRenderer>().color = ((moves[i][0] + moves[i][1]) % 2 == 0) ? highlightedDarkSquare : highlightedLightSquare;
        }
    }

    public void MovePiece(int[] from, int[] to) => Board.pieces.FirstOrDefault(x => x.position == from).gameobject.transform.position = new Vector3((to[0] - 3.5f) * squareSize, (to[1] - 3.5f) * squareSize, 0f);

    public GameObject UpgradeSelectedPiece(int newPiece)
    {
        int pieceType = newPiece;
        int pieceColor = Board.selectedPiece.type & 24;
        int index = pieceType + 6 * (pieceColor == ChessPieceTypes.White? 0 : 1) - 1;
        int[] pos = Board.selectedPiece.position;

        Destroy(Board.selectedPiece.gameobject);
        
        return Instantiate(piecesPrefabs[index], new Vector3((pos[0] - 3.5f) * squareSize, (pos[1] - 3.5f) * squareSize, 0f), Quaternion.identity, transform.GetChild(0));
    }

    void ReadFEN()
    {
        FENReading fen = Essentials.ReadFEN(FEN);
        Board.pieces = fen.pieces;
        Board.turnToMove = fen.turn;
        Board.enPassantSquare = fen.enPassantTS;

        for (int i = 0; i < Board.pieces.Count; i++)
        {
            Board.board.Add(new int[2] { Board.pieces[i].position[0], Board.pieces[i].position[1] }, Board.pieces[i]);
        }
    }

    void CreateBoard()
    {
        squares = new GameObject[64];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject square = Instantiate(squarePrefab, new Vector3((j - 3.5f) * squareSize, (i - 3.5f) * squareSize, .1f), Quaternion.identity, transform.GetChild(1));
                square.GetComponent<SpriteRenderer>().color = (i + j) % 2 == 0 ? darkSquare : lightSquare;
                square.transform.localScale = Vector3.one * squareSize;
                square.GetComponent<ChessSquare>().pos = new int[2] { j, i };

                squares[i * 8 + j] = square;
            }
        }
    }

    void CreatePieces()
    {
        for (int i = 0; i < Board.pieces.Count; i++)
        {
            int[] pos = Board.pieces[i].position;

            int pieceType = Board.pieces[i].type & 7;
            int pieceColor = Board.pieces[i].type & 24;
            int index = pieceType + 6 * (pieceColor == ChessPieceTypes.White? 0 : 1) - 1;

            var piece = Instantiate(piecesPrefabs[index], new Vector3((pos[0] - 3.5f) * squareSize, (pos[1] - 3.5f) * squareSize, 0f), Quaternion.identity, transform.GetChild(0));
            piece.GetComponent<ChessPiece>().identity = Board.pieces[i];
            piece.GetComponent<ChessPiece>().identity.gameobject = Board.pieces[i].gameobject = piece;
        }
    }
}
