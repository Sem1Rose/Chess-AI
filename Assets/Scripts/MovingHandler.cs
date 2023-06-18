using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MovingHandler : MonoBehaviour
{
    #region Singleton
    public static MovingHandler handler = null;

    void Awake()
    {
        if (handler != null)
            Debug.LogError("More than one Moving handler in the scene!", gameObject);
        handler = this;
    }
    #endregion

    public static Move MakeMove(ref Piece selectedPiece, int[] moveTo, ref Piece enPassantPiece, ref int[] enPassantSquare, ref bool capturing, ref Piece capturedPiece, ref Dictionary<int[], Piece> board, ref List<Piece> pieces, bool updateGraphics = true)
    {
        Move move = null;
        int[] moveFrom;
        bool moved;
        bool castled = false;
        Piece castlingRook = null;
        bool enPassant = false;
        int[] enPass = enPassantSquare;
        bool upgraded = false;

        if (selectedPiece != null && moveTo != null && !capturing)
        {
            List<int[]> moves = Essentials.GenerateLegalMoves(selectedPiece);

            var temp1 = moveTo;

            if (moves.Any(x => x.SequenceEqual(temp1)))
            {
                CheckEnPassant(ref selectedPiece, ref moveTo, ref enPassantPiece, ref enPassantSquare, ref board, ref pieces, updateGraphics, ref enPassant);
                CheckPawnUpgrade(ref selectedPiece, ref moveTo, ref board, ref pieces, updateGraphics, ref upgraded);
                CheckKingCastling(ref selectedPiece, ref moveTo, ref board, updateGraphics, ref castled, ref castlingRook);

                moveFrom = selectedPiece.position;
                moved = selectedPiece.moved;
                MovePiece(ref selectedPiece, ref moveTo, ref board, updateGraphics);

                Essentials.ChangeTurn();
                move = new Move(selectedPiece, moveFrom, moveTo, moved, false, null, upgraded, enPassant, enPass, castled, castlingRook, board, pieces);
            }

            selectedPiece = null;
            moveTo = null;
        }
        else if (selectedPiece != null && moveTo != null && capturing)
        {
            List<int[]> moves = Essentials.GenerateLegalMoves(selectedPiece);

            enPassantSquare = null;
            enPassantPiece = null;

            var temp1 = moveTo;

            if (moves.Any(x => x.SequenceEqual(temp1)))
            {
                var temp2 = capturedPiece;
                pieces.Remove(pieces.FirstOrDefault(x => x == temp2));
                board.Remove(board.FirstOrDefault(x => x.Key.SequenceEqual(temp2.position)).Key);
                if (updateGraphics)
                    Destroy(capturedPiece.gameobject);

                moveFrom = selectedPiece.position;
                moved = selectedPiece.moved;
                MovePiece(ref selectedPiece, ref moveTo, ref board, updateGraphics);

                CheckPawnUpgrade(ref selectedPiece, ref moveTo, ref board, ref pieces, updateGraphics, ref upgraded);

                Essentials.ChangeTurn();
                move = new Move(selectedPiece, moveFrom, moveTo, moved, capturing, capturedPiece, upgraded, false, null, false, null, board, pieces);
            }

            selectedPiece = null;
            moveTo = null;
            capturedPiece = null;
            capturing = false;
        }
        return move;
    }

    static void MovePiece(ref Piece selectedPiece, ref int[] moveTo, ref Dictionary<int[], Piece> board, bool updateGraphics, bool moved = true)
    {
        if (updateGraphics)
            GraphicsHandler.handler.MovePiece(selectedPiece.position, moveTo);

        var temp1 = selectedPiece;
        board.Remove(board.FirstOrDefault(x => x.Key.SequenceEqual(temp1.position)).Key);
        selectedPiece.position = moveTo;
        selectedPiece.moved = moved;
        board.Add(moveTo, selectedPiece);
    }

    static void CheckEnPassant(ref Piece selectedPiece, ref int[] moveTo, ref Piece enPassantPiece, ref int[] enPassantSquare, ref Dictionary<int[], Piece> board, ref List<Piece> pieces, bool updateGraphics, ref bool enPassant)
    {
        if (enPassantSquare != null && moveTo.SequenceEqual(enPassantSquare))
        {
            var temp1 = enPassantPiece;
            pieces.Remove(pieces.FirstOrDefault(x => x == temp1));
            board.Remove(board.FirstOrDefault(x => x.Key.SequenceEqual(temp1.position)).Key);
            if (updateGraphics)
                Destroy(enPassantPiece.gameobject);
            enPassant = true;
        }

        enPassantSquare = null;
        enPassantPiece = null;

        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.Pawn))
        {
            if (Mathf.Abs(selectedPiece.position[1] - moveTo[1]) == 2)
            {
                enPassantSquare = new int[2] { moveTo[0], selectedPiece.position[1] + (Essentials.CheckColor(selectedPiece, ChessPieceTypes.White) ? 1 : -1) };
                enPassantPiece = selectedPiece;
            }
        }
    }

    static void CheckKingCastling(ref Piece selectedPiece, ref int[] moveTo, ref Dictionary<int[], Piece> board, bool updateGraphics, ref bool castled, ref Piece castlingRook)
    {
        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.king) && Mathf.Abs(selectedPiece.position[0] - moveTo[0]) == 2)
        {
            bool right = moveTo[0] - selectedPiece.position[0] == 2;
            var temp1 = selectedPiece;
            castlingRook = board.FirstOrDefault(x => x.Key.SequenceEqual(new int[2] { (right ? 7 : 0), (Essentials.CheckColor(temp1, ChessPieceTypes.White) ? 0 : 7) })).Value;

            if (castlingRook != null)
            {
                castled = true;

                int[] castlingSquare = new int[2] { selectedPiece.position[0] + (right ? 1 : -1), selectedPiece.position[1] };

                if (updateGraphics)
                    GraphicsHandler.handler.MovePiece(castlingRook.position, castlingSquare);

                var temp = castlingRook;
                board.Remove(board.FirstOrDefault(x => x.Key.SequenceEqual(temp.position)).Key);
                castlingRook.position = castlingSquare;
                castlingRook.moved = true;
                board.Add(castlingSquare, castlingRook);
                castlingSquare = null;
            }
        }
    }

    static void CheckPawnUpgrade(ref Piece selectedPiece, ref int[] moveTo, ref Dictionary<int[], Piece> board, ref List<Piece> pieces, bool updateGraphics, ref bool upgraded)
    {
        if (Essentials.CheckType(selectedPiece, ChessPieceTypes.Pawn) && moveTo[1] == (Essentials.CheckColor(selectedPiece, ChessPieceTypes.White) ? 7 : 0))
        {
            int newType = ChessPieceTypes.Queen;
            Piece oldPiece = selectedPiece;

            pieces.Remove(pieces.FirstOrDefault(x => x.position.SequenceEqual(oldPiece.position)));
            board.Remove(board.FirstOrDefault(x => x.Key.SequenceEqual(oldPiece.position)).Key);

            Piece newPiece = new Piece(Essentials.GetColor(oldPiece) | newType, oldPiece.position);
            pieces.Add(newPiece);
            board.Add(oldPiece.position, newPiece);
            selectedPiece = newPiece;

            if (updateGraphics)
                GraphicsHandler.handler.UpgradePiece(oldPiece, newPiece, newType);
            upgraded = true;
        }
    }
    //Piece selectedPiece, int[] from, int[] to, bool capturing, Piece capturedPiece, bool upgraded, bool enPassant, int[] enPassantSquare, bool castled, Piece castlingRook, Dictionary<int[], Piece> board, List<Piece> pieces
    public static Move UndoMove(Move move, bool updateGraphics)
    {
        Move undoMove = move;

        MovePiece(ref undoMove.selectedPiece, ref undoMove.from, ref undoMove.board, updateGraphics, move.moved);

        if (move.capturing)
        {
            undoMove.board.Add(move.capturedPiece.position, move.capturedPiece);
            undoMove.pieces.Add(move.capturedPiece);

            if (updateGraphics)
                GraphicsHandler.handler.MakePiece(move.capturedPiece);
        }
        if (move.upgraded)
        {
            int newType = ChessPieceTypes.Pawn;
            Piece oldPiece = undoMove.selectedPiece;

            undoMove.pieces.Remove(undoMove.pieces.FirstOrDefault(x => x == oldPiece));
            undoMove.board.Remove(undoMove.board.FirstOrDefault(x => x.Key.SequenceEqual(oldPiece.position)).Key);

            Piece newPiece = new Piece(Essentials.GetColor(oldPiece) | newType, undoMove.from);
            undoMove.pieces.Add(newPiece);
            undoMove.board.Add(undoMove.from, newPiece);

            if (updateGraphics)
                GraphicsHandler.handler.UpgradePiece(oldPiece, newPiece, newType);
        }
        if (move.enPassant)
        {
            bool white = Essentials.CheckColor(move.selectedPiece, ChessPieceTypes.White);
            int[] enPassantPos = Essentials.Move(move.enPassantSquare, white ? 1 : 0);

            Piece newPiece = new Piece(ChessPieceTypes.Pawn | (white ? ChessPieceTypes.Black : ChessPieceTypes.White), enPassantPos, true);

            move.pieces.Add(newPiece);
            move.board.Add(enPassantPos, newPiece);

            if (updateGraphics)
                GraphicsHandler.handler.MakePiece(newPiece);
        }
        if (move.castled)
        {
            bool kingSide = move.to[0] == 6;
            Debug.Log(kingSide);
            bool white = Essentials.CheckColor(move.selectedPiece, ChessPieceTypes.White);
            int[] rookNewPos = new int[2] { (kingSide ? 7 : 0), (white ? 0 : 7) };

            MovePiece(ref move.castlingRook, ref rookNewPos, ref move.board, updateGraphics, false);
        }

        Essentials.ChangeTurn();
        return undoMove;
    }

    public static void ApplyMove(Move undoneMove)
    {
        Board.enPassantSquare = undoneMove.enPassantSquare;
        if (Board.enPassantSquare != null)
        {
            Board.enPassantPiece = Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(Essentials.Move(undoneMove.enPassantSquare, (undoneMove.enPassantSquare[1] == 5 ? 1 : 0))));
        }
        Board.board = undoneMove.board;
        Board.pieces = undoneMove.pieces;
    }
}
