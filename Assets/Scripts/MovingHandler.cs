using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MovingHandler : MonoBehaviour
{
    public static MovingHandler handler = null;

    void Awake()
    {
        if(handler != null)
            Debug.LogError("More than one Moving handler in the scene!", gameObject);
        handler = this;
    }

    public static void Move()
    {
        if (Board.selectedPiece != null && Board.selectedSquare != null && !Board.capturing)
        {
            List<int[]> moves = Essentials.GenerateMoves(Board.selectedPiece);

            if (moves.Any(x => x.SequenceEqual(Board.selectedSquare)))
            {
                if(Board.enPassantSquare != null && Board.selectedSquare.SequenceEqual(Board.enPassantSquare))
                {
                    Board.pieces.Remove(Board.pieces.FirstOrDefault(x => x == Board.enPassantPiece));
                    Board.board.Remove(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(Board.enPassantPiece.position)).Key);
                    Destroy(Board.enPassantPiece.gameobject);
                }

                Board.enPassantSquare = null;
                Board.enPassantPiece = null;

                if (Essentials.CheckType(Board.selectedPiece, ChessPieceTypes.Pawn))
                {
                    if (Mathf.Abs(Board.selectedPiece.position[1] - Board.selectedSquare[1]) == 2)
                    {
                        Board.enPassantSquare = new int[2] { Board.selectedSquare[0], Board.selectedPiece.position[1] + (Essentials.CheckColor(Board.selectedPiece, ChessPieceTypes.White)? 1 : -1) };
                        Board.enPassantPiece = Board.selectedPiece;
                    }
                }

                if(Essentials.CheckType(Board.selectedPiece, ChessPieceTypes.Pawn) && Board.selectedSquare[1] == (Essentials.CheckColor(Board.selectedPiece, ChessPieceTypes.White)? 7 : 0))
                {
                    int newType = Random.Range(3, 6);
                    Piece oldPiece = Board.selectedPiece;
                    GameObject newPiece = GraphicsHandler.handler.UpgradeSelectedPiece(newType);

                    Board.selectedPiece = null;
                    Board.pieces.Remove(Board.pieces.FirstOrDefault(x => x == oldPiece));
                    Board.board.Remove(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(oldPiece.position)).Key);

                    Piece n = new Piece(Essentials.GetColor(oldPiece) | newType, oldPiece.position, newPiece);
                    Board.pieces.Add(n);
                    Board.board.Add(oldPiece.position, n);
                    Board.selectedPiece = n;

                    newPiece.gameObject.GetComponent<ChessPiece>().identity = n;
                }

                GraphicsHandler.handler.MovePiece(Board.selectedPiece.position, Board.selectedSquare);
                Board.board.Remove(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(Board.selectedPiece.position)).Key);
                Board.selectedPiece.position = Board.selectedSquare;
                Board.selectedPiece.moved = true;
                Board.board.Add(Board.selectedSquare, Board.selectedPiece);

                Essentials.ChangeTurn();
            }

            Board.selectedPiece = null;
            Board.selectedSquare = null;
        }
        else if (Board.selectedPiece != null && Board.selectedSquare != null && Board.capturing)
        {
            List<int[]> moves = Essentials.GenerateMoves(Board.selectedPiece);

            Board.enPassantSquare = null;
            Board.enPassantPiece = null;

            if (moves.Any(x => x.SequenceEqual(Board.selectedSquare)))
            {
                if(Essentials.CheckType(Board.selectedPiece, ChessPieceTypes.Pawn) && Board.selectedSquare[1] == (Essentials.CheckColor(Board.selectedPiece, ChessPieceTypes.White)? 7 : 0))
                {
                    int newType = Random.Range(3, 6);
                    Piece oldPiece = Board.selectedPiece;
                    GameObject newPiece = GraphicsHandler.handler.UpgradeSelectedPiece(newType);

                    Board.selectedPiece = null;
                    Board.pieces.Remove(Board.pieces.FirstOrDefault(x => x == oldPiece));
                    Board.board.Remove(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(oldPiece.position)).Key);

                    Piece n = new Piece(Essentials.GetColor(oldPiece) | newType, oldPiece.position, newPiece);
                    Board.pieces.Add(n);
                    Board.board.Add(oldPiece.position, n);
                    Board.selectedPiece = n;

                    newPiece.gameObject.GetComponent<ChessPiece>().identity = n;
                }

                Board.pieces.Remove(Board.pieces.FirstOrDefault(x => x == Board.capturedPiece));
                Board.board.Remove(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(Board.capturedPiece.position)).Key);
                Destroy(Board.capturedPiece.gameobject);

                GraphicsHandler.handler.MovePiece(Board.selectedPiece.position, Board.selectedSquare);
                Board.board.Remove(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(Board.selectedPiece.position)).Key);
                Board.selectedPiece.position = Board.selectedSquare;
                Board.selectedPiece.moved = true;
                Board.board.Add(Board.selectedSquare, Board.selectedPiece);

                Essentials.ChangeTurn();
            }

            Board.selectedPiece = null;
            Board.selectedSquare = null;
            Board.capturedPiece = null;
            Board.capturing = false;
        }
    }
}
