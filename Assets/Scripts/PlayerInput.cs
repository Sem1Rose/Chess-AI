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
                GraphicsHandler.handler.ResetBoard();

                Board.selectedPiece = null;
                Board.selectedSquare = null;
                Board.capturing = false;
                Board.capturedPiece = null;
            }
            else if (hit.collider.gameObject.CompareTag("Piece") && Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
            {
                GraphicsHandler.handler.ResetBoard();

                Board.selectedPiece = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                Board.capturedPiece = null;
                Board.selectedSquare = null;
                Board.capturing = false;

                Board.generatedMoves = MovesGenerator.GenerateLegalMoves(Board.selectedPiece);

                GraphicsHandler.handler.HighlightMoves();
            }
            else if (hit.collider.gameObject.CompareTag("Piece") && !Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
            {
                Piece capture = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                if (Board.selectedPiece != null && Board.generatedMoves.Any(x => x.SequenceEqual(capture.position)))
                {
                    Board.capturedPiece = capture;
                    Board.selectedSquare = capture.position;
                    Board.capturing = true;
                }
                else
                {
                    Board.selectedPiece = null;
                    Board.selectedSquare = null;
                    Board.capturing = false;
                    Board.capturedPiece = null;
                }

                GraphicsHandler.handler.ResetBoard();
            }
            else if (hit.collider.gameObject.CompareTag("Square"))
            {
                int[] selectedSquare = hit.collider.gameObject.GetComponent<ChessSquare>().pos;
                if (Board.selectedPiece != null && Board.generatedMoves.Any(x => x.SequenceEqual(selectedSquare)))
                {
                    Board.selectedSquare = selectedSquare;
                    Board.capturedPiece = null;
                    Board.capturing = false;
                }
                else
                {
                    Board.selectedPiece = null;
                    Board.selectedSquare = null;
                    Board.capturing = false;
                    Board.capturedPiece = null;
                }

                GraphicsHandler.handler.ResetBoard();
            }

            if (Board.selectedPiece != null && Board.selectedSquare != null)
            {
                Move move = MovingHandler.MakeMove(Board.selectedPiece, Board.selectedSquare, true);
                Board.selectedPiece = null;
                Board.selectedSquare = null;

                if (move != null)
                    Board.lastMove = move;
            }

        }
        if (Input.GetKeyDown(KeyCode.Space) && Board.lastMove != null)
        {
            GraphicsHandler.handler.ResetBoard();
            MovingHandler.UndoMove(Board.lastMove, true);

            Board.lastMove = null;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GraphicsHandler.handler.ResetBoard();
            Essentials.ChangeTurn();
        }

    }
}
