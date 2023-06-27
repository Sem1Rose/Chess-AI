namespace Chess
{
    using UnityEngine;
    using System.Linq;

    public class PlayerInputHandler : MonoBehaviour
    {
        void Update()
        {
            if (Board.gameEnded)
                return;
            if (Board.gameMode == GameModes.PvB && Board.turnToMove != ChessPieceTypes.White)
                return;
            else if (Board.gameMode == GameModes.BvB)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider == null)
                {
                    GraphicsHandler.handler.ResetBoard(false);

                    Board.selectedPiece = null;
                    Board.selectedSquare = null;
                    Board.capturing = false;
                    Board.capturedPiece = null;
                }
                else if (hit.collider.gameObject.CompareTag("Piece") && Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
                {
                    GraphicsHandler.handler.ResetBoard(false);

                    Board.selectedPiece = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                    Board.capturedPiece = null;
                    Board.selectedSquare = null;
                    Board.capturing = false;

                    Board.generatedMoves = MovesGenerator.GenerateLegalMoves(Board.selectedPiece);

                    GraphicsHandler.handler.HighlightGeneratedMoves();
                }
                else if (hit.collider.gameObject.CompareTag("Piece") && !Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
                {
                    Piece capture = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                    if (Board.selectedPiece != null && Board.generatedMoves.Any(x => x.SequenceEqual(capture.position)))
                    {
                        Board.capturedPiece = capture;
                        Board.selectedSquare = capture.position;
                        Board.capturing = true;

                        GraphicsHandler.handler.ResetBoard(true);
                        GraphicsHandler.handler.HighlightMove(Board.selectedPiece.position, Board.selectedSquare);
                    }
                    else
                    {
                        GraphicsHandler.handler.ResetBoard(false);

                        Board.selectedPiece = null;
                        Board.selectedSquare = null;
                        Board.capturing = false;
                        Board.capturedPiece = null;
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Square"))
                {

                    int[] selectedSquare = hit.collider.gameObject.GetComponent<ChessSquare>().pos;
                    if (Board.selectedPiece != null && Board.generatedMoves.Any(x => x.SequenceEqual(selectedSquare)))
                    {
                        Board.selectedSquare = selectedSquare;
                        Board.capturedPiece = null;
                        Board.capturing = false;

                        GraphicsHandler.handler.ResetBoard(true);
                        GraphicsHandler.handler.HighlightMove(Board.selectedPiece.position, Board.selectedSquare);
                    }
                    else
                    {
                        GraphicsHandler.handler.ResetBoard(false);

                        Board.selectedPiece = null;
                        Board.selectedSquare = null;
                        Board.capturing = false;
                        Board.capturedPiece = null;
                    }
                }

                if (Board.selectedPiece != null && Board.selectedSquare != null)
                {
                    Move move = MovingHandler.MakeMove(Board.selectedPiece, Board.selectedSquare, true);
                    Board.selectedPiece = null;
                    Board.selectedSquare = null;

                    if (move != null)
                    {
                        Board.lastMove = move;
                        GameHandler.handler.UpdateUI();
                    }
                }

            }
            if (Input.GetKeyDown(KeyCode.Space) && Board.lastMove != null)
            {
                GraphicsHandler.handler.ResetBoard(true);
                MovingHandler.UndoMove(Board.lastMove, true);

                Board.lastMove = null;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                GraphicsHandler.handler.ResetBoard(false);
                Essentials.ChangeTurn();
            }

        }
    }
}