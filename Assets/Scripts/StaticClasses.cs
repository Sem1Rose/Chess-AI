namespace Chess
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Extensions
    {
        static bool SamePositiveDiagonal(this int[] pos, int[] other) => (float)(pos[1] - other[1]) / (float)(pos[0] - other[0]) == 1f;
        static bool SameNegativeDiagonal(this int[] pos, int[] other) => (float)(pos[1] - other[1]) / (float)(pos[0] - other[0]) == -1f;

        public static bool SameCross(this int[] pos, int[] other, int fileRank, int positiveNegative)
        {
            return pos[fileRank] == other[fileRank] && (positiveNegative == 0 ? other[fileRank == 0 ? 1 : 0] - pos[fileRank == 0 ? 1 : 0] > 0 : other[fileRank == 0 ? 1 : 0] - pos[fileRank == 0 ? 1 : 0] < 0);
        }

        public static bool SameDiagonal(this int[] pos, int[] other, int upDown, int rightLeft)
        {
            if (!(upDown == rightLeft ? SamePositiveDiagonal(pos, other) : SameNegativeDiagonal(pos, other)))
                return false;

            return (upDown == 0 ? (other[1] - pos[1]) > 0 : (other[1] - pos[1]) < 0) && (rightLeft == 0 ? (other[0] - pos[0]) > 0 : (other[0] - pos[0]) < 0);
        }
    }

    public static class Board
    {
        public enum GameEndReason
        {
            CheckMate,
            StaleMate,
            Draw
        }
        public static GameEndReason gameEndReason;
        public static GameModes gameMode;

        public static List<int[]> opponentThreatMap;
        public static List<int[]> checkSquares = new List<int[]>();
        public static List<int[]> generatedMoves;
        public static List<Piece> pieces = new List<Piece>();
        public static List<Piece> pinnedPieces = new List<Piece>();
        public static List<Piece> checkingPieces = new List<Piece>();
        public static Move lastMove = null;

        public static Piece whiteKing
        {
            get
            {
                return pieces.FirstOrDefault(x => x.type == (ChessPieceTypes.king | ChessPieceTypes.White));
            }
        }
        public static Piece blackKing
        {
            get
            {
                return pieces.FirstOrDefault(x => x.type == (ChessPieceTypes.king | ChessPieceTypes.Black));
            }
        }
        public static Piece selectedPiece = null;
        public static Piece capturedPiece = null;
        public static Piece enPassantPiece = null;
        public static Piece castlingRook = null;

        public static int[] selectedSquare = null;
        public static int[] enPassantSquare = null;
        public static int turnToMove = ChessPieceTypes.White;
        public static int numCheckers;

        public static bool gameEnded = false;
        public static bool capturing = false;
        public static bool[] whiteCastlingRights; // Ks AND Qs
        public static bool[] blackCastlingRights; // Ks AND Qs

        public static string gameFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    }

    [System.Serializable]
    public class Piece
    {
        public int type;
        public int[] position;
        public bool moved;
        public GameObject pieceObject;

        public Piece(int type, int[] position, GameObject pieceObject, bool moved = false)
        {
            this.type = type;
            this.position = position;
            this.pieceObject = pieceObject;
            this.moved = moved;
        }
        public Piece(int type, int[] position, bool moved = false)
        {
            this.type = type;
            this.position = position;
            this.moved = moved;
        }
    }

    public class Move
    {
        public Piece selectedPiece;
        public int[] from;
        public int[] to;
        public bool moved;
        public bool capturing;
        public Piece capturedPiece;
        public bool upgraded;
        public Piece upgradedPiece;
        public bool enPassant;
        public int[] enPassantSquare;
        public Piece enPassantPiece;
        public bool castled;
        public bool kingSide;
        public Piece castlingRook;
        public List<Piece> pieces;
        //public Dictionary<int[], Piece> board;

        public Move(Piece selectedPiece, int[] from, int[] to, bool moved, bool capturing, Piece capturedPiece, bool upgraded, Piece upgradedPiece, bool enPassant, int[] enPassantSquare, Piece enPassantPiece, bool castled, bool kingSide, Piece castlingRook/*, Dictionary<int[], Piece> board*/, List<Piece> pieces)
        {
            this.selectedPiece = selectedPiece;
            this.from = from;
            this.to = to;
            this.moved = moved;
            this.capturing = capturing;
            this.capturedPiece = capturedPiece;
            this.upgraded = upgraded;
            this.upgradedPiece = upgradedPiece;
            this.enPassant = enPassant;
            this.enPassantSquare = enPassantSquare;
            this.enPassantPiece = enPassantPiece;
            this.castled = castled;
            this.kingSide = kingSide;
            this.castlingRook = castlingRook;
            this.pieces = pieces;
        }
    }

    public static class ChessPieceTypes
    {
        public const int None = 0;
        public const int Pawn = 1;
        public const int king = 2;
        public const int Bishop = 3;
        public const int Knight = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        public static int White = 8;
        public static int Black = 16;
    }

    public class FENReading
    {
        public List<Piece> pieces;
        public int turn;
        public bool[] whiteCastlingRights;
        public bool[] blackCastlingRights;
        public int[] enPassantTS;
        public int halfMoveC;
        public int fullMoveC;

        public FENReading(List<Piece> pieces, int turn, bool[] whiteCastlingRights, bool[] blackCastlingRights, int[] enPassantTS, int halfMoveC, int fullMoveC)
        {
            this.pieces = pieces;
            this.turn = turn;
            this.whiteCastlingRights = whiteCastlingRights;
            this.blackCastlingRights = blackCastlingRights;
            this.enPassantTS = enPassantTS;
            this.halfMoveC = halfMoveC;
            this.fullMoveC = fullMoveC;
        }
    }

    public enum GameModes
    {
        PvP, PvB, BvB
    }
}