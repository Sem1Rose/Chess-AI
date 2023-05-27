using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    void Update()
    {
        if(GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvB && Board.turnToMove != ChessPieceTypes.White)
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
            }
            else if(hit.collider.gameObject.CompareTag("Piece") && !Essentials.CheckColor(hit.collider.gameObject.GetComponent<ChessPiece>().identity, Board.turnToMove))
            {
                Board.selectedSquare = hit.collider.gameObject.GetComponent<ChessPiece>().identity.position;
                Board.capturedPiece = hit.collider.gameObject.GetComponent<ChessPiece>().identity;
                Board.capturing = true;

                GraphicsHandler.handler.ResetBoard();
            }
            else if(hit.collider.gameObject.CompareTag("Square")) 
            {
                Board.selectedSquare = hit.collider.gameObject.GetComponent<ChessSquare>().pos;
                Board.capturedPiece = null;
                Board.capturing = false;

                GraphicsHandler.handler.ResetBoard();
            }
            MovingHandler.HandleMovement();
        }
    }
}
