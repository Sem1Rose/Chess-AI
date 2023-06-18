using UnityEngine;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    void Update()
    {
        if (GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvB && Board.turnToMove != ChessPieceTypes.White)
            return;
        else if (GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.BvB)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider == null)
            {
                if (Board.selectedPiece != null)
                {
                    GraphicsHandler.handler.ResetBoard();
                }

                Board.selectedSquare = null;
                Board.selectedPiece = null;
                Board.capturedPiece = null;
                Board.capturing = false;
            }
            else if (hit.collider.gameObject.CompareTag("Piece") && Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
            {
                if (Board.selectedPiece != null)
                {
                    GraphicsHandler.handler.ResetBoard();
                }

                Board.selectedPiece = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                Board.capturedPiece = null;
                Board.selectedSquare = null;
                Board.capturing = false;

                GraphicsHandler.handler.HighlightMove();
                Essentials.ChangeTurn();
                Essentials.GenerateThreatMap(Board.turnToMove);
                Essentials.ChangeTurn();
                Debug.Log(Board.checkPieces.Count);
            }
            else if (hit.collider.gameObject.CompareTag("Piece") && !Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
            {
                Board.selectedSquare = hit.collider.gameObject.GetComponent<ChessPiece>().identity.position;
                Board.capturedPiece = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                Board.capturing = true;

                GraphicsHandler.handler.ResetBoard();
            }
            else if (hit.collider.gameObject.CompareTag("Square"))
            {
                Board.selectedSquare = hit.collider.gameObject.GetComponent<ChessSquare>().pos;
                Board.capturedPiece = null;
                Board.capturing = false;

                GraphicsHandler.handler.ResetBoard();
            }
            Move move = MovingHandler.MakeMove(ref Board.selectedPiece, Board.selectedSquare, ref Board.enPassantPiece, ref Board.enPassantSquare, ref Board.capturing, ref Board.capturedPiece, ref Board.board, ref Board.pieces);

            if (move != null)
                Board.lastMove = move;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Board.lastMove != null)
        {
            Move undoneMove = MovingHandler.UndoMove(Board.lastMove, true);
            Board.enPassantSquare = undoneMove.enPassantSquare;
            if (Board.enPassantSquare != null)
            {
                Board.enPassantPiece = Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(Essentials.Move(undoneMove.enPassantSquare, (undoneMove.enPassantSquare[1] == 5 ? 1 : 0))));
            }
            Board.board = undoneMove.board;
            Board.pieces = undoneMove.pieces;
            Board.lastMove = null;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Essentials.ChangeTurn();

    }
}
