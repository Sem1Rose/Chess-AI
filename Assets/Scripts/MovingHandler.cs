using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MovingHandler : MonoBehaviour
{
    #region Singleton
    public static MovingHandler handler;

    void Awake()
    {
        if (handler != null)
            Debug.LogError("More than one Moving handler in the scene!", gameObject);
        handler = this;
    }
    #endregion

    static bool upgraded, castled, enPassant;

    public static Move MakeMove(Piece selectedPiece, int[] moveTo, bool updateGraphics = true)
    {
        Move move = null;

        bool moved;
        int[] moveFrom;
        int[] enPassantSquare = Board.enPassantSquare;
        Piece selected = selectedPiece;
        Piece enPassantPiece = Board.enPassantPiece;
        List<Piece> pieces = Board.pieces;

        if (selectedPiece != null && moveTo != null && !Board.capturing)
        {
            moveFrom = selectedPiece.position;
            moved = selectedPiece.moved;

            CheckKingCastling(selectedPiece, moveTo, updateGraphics);
            selectedPiece = MovePiece(selectedPiece, moveTo, updateGraphics);
            selectedPiece = CheckPawnUpgrade(selectedPiece, moveTo, updateGraphics);
            CheckEnPassant(selectedPiece, moveTo, updateGraphics);

            Essentials.ChangeTurn();

            move = new Move(selected, moveFrom, moveTo, moved, false, null, upgraded, upgraded ? selectedPiece : null, enPassant, enPassantSquare, enPassantPiece, castled, Board.castlingRook, pieces);
        }
        else if (selectedPiece != null && moveTo != null && Board.capturing)
        {
            Piece capturedPiece = Board.capturedPiece;

            Board.enPassantSquare = null;
            Board.enPassantPiece = null;

            Board.pieces.Remove(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(Board.capturedPiece.position)));

            if (updateGraphics)
                GraphicsHandler.handler.DestroyPiece(Board.capturedPiece.pieceObject);

            selectedPiece = CheckPawnUpgrade(selectedPiece, moveTo, updateGraphics);

            moveFrom = selectedPiece.position;
            moved = selectedPiece.moved;
            selectedPiece = MovePiece(selectedPiece, moveTo, updateGraphics);

            Essentials.ChangeTurn();
            move = new Move(selected, moveFrom, moveTo, moved, Board.capturing, capturedPiece, upgraded, upgraded ? selectedPiece : null, false, enPassantSquare, enPassantPiece, false, null, pieces);

            Board.capturedPiece = null;
            Board.capturing = false;
        }

        return move;
    }

    static Piece MovePiece(Piece selectedPiece, int[] moveTo, bool updateGraphics, bool moved = true)
    {
        Board.pieces.RemoveAll(x => x.position.SequenceEqual(selectedPiece.position));
        selectedPiece.position = moveTo;
        selectedPiece.moved = moved;
        Board.pieces.Add(selectedPiece);
        if (updateGraphics)
            GraphicsHandler.handler.MovePiece(selectedPiece.position, moveTo);

        return selectedPiece;
    }

    public static Move UndoMove(Move move, bool updateGraphics)
    {
        move.selectedPiece = MovePiece(move.selectedPiece, move.from, updateGraphics, move.moved);

        if (move.upgraded)
        {
            int newType = ChessPieceTypes.Pawn;
            Piece oldPiece = move.upgradedPiece;

            Board.pieces.RemoveAll(x => x.position.SequenceEqual(oldPiece.position));
            Piece newPiece = new Piece(Essentials.GetColor(oldPiece) | newType, move.from);
            Board.pieces.Add(newPiece);

            if (updateGraphics)
                GraphicsHandler.handler.UpgradePiece(oldPiece, newPiece, newType);
        }
        if (move.capturing)
        {
            Board.pieces.Add(move.capturedPiece);

            if (updateGraphics)
                GraphicsHandler.handler.MakePiece(move.capturedPiece);

            Board.capturing = false;
            Board.capturedPiece = null;
        }
        if (move.enPassant)
        {
            Board.pieces.Add(move.enPassantPiece);
            GraphicsHandler.handler.MakePiece(move.enPassantPiece);
        }
        if (move.castled)
        {
            bool kingSide = move.to[0] == 6;
            bool white = Essentials.CheckColor(move.selectedPiece, ChessPieceTypes.White);
            int[] rookNewPos = new int[2] { (kingSide ? 7 : 0), (white ? 0 : 7) };

            move.castlingRook = MovePiece(move.castlingRook, rookNewPos, updateGraphics, false);
        }

        Board.enPassantSquare = move.enPassantSquare;
        Board.enPassantPiece = move.enPassantPiece;
        Essentials.ChangeTurn();

        return move;
    }

    static Piece CheckPawnUpgrade(Piece selectedPiece, int[] moveTo, bool updateGraphics)
    {
        upgraded = false;
        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.Pawn) && moveTo[1] == (Essentials.CheckColor(selectedPiece, ChessPieceTypes.White) ? 7 : 0))
        {
            int newType = ChessPieceTypes.Queen;
            Piece oldPiece = selectedPiece;

            Board.pieces.RemoveAll(x => x.position.SequenceEqual(oldPiece.position));
            Piece newPiece = new Piece(Essentials.GetColor(oldPiece) | newType, oldPiece.position);
            selectedPiece = newPiece;
            Board.pieces.Add(selectedPiece);

            if (updateGraphics)
                GraphicsHandler.handler.UpgradePiece(oldPiece, newPiece, newType);

            upgraded = true;
        }
        return selectedPiece;
    }

    static void CheckEnPassant(Piece selectedPiece, int[] moveTo, bool updateGraphics)
    {
        enPassant = false;
        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.Pawn) && Board.enPassantSquare != null && moveTo.SequenceEqual(Board.enPassantSquare))
        {
            Board.pieces.RemoveAll(x => x.position.SequenceEqual(Board.enPassantPiece.position));
            GraphicsHandler.handler.DestroyPiece(Board.enPassantPiece.pieceObject);
            enPassant = true;
        }

        Board.enPassantSquare = null;
        Board.enPassantPiece = null;

        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.Pawn))
        {
            if (Mathf.Abs(selectedPiece.position[1] - moveTo[1]) == 2)
            {
                Board.enPassantSquare = new int[2] { moveTo[0], selectedPiece.position[1] + (Essentials.CheckColor(selectedPiece, ChessPieceTypes.White) ? 1 : -1) };
                Board.enPassantPiece = selectedPiece;
            }
        }
    }

    static void CheckKingCastling(Piece selectedPiece, int[] moveTo, bool updateGraphics)
    {
        castled = false;
        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.king) && Mathf.Abs(selectedPiece.position[0] - moveTo[0]) == 2)
        {
            bool right = moveTo[0] - selectedPiece.position[0] == 2;
            Board.castlingRook = Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(new int[2] { (right ? 7 : 0), (Essentials.CheckColor(selectedPiece, ChessPieceTypes.White) ? 0 : 7) }));

            if (Board.castlingRook != null)
            {
                int[] castlingSquare = new int[2] { selectedPiece.position[0] + (right ? 1 : -1), selectedPiece.position[1] };
                Board.castlingRook = MovePiece(Board.castlingRook, castlingSquare, updateGraphics, updateGraphics);
                castled = true;
            }
        }
    }
}
