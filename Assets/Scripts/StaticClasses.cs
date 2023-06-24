using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Board
{
    public static List<int[]> playerThreatMap;
    public static List<int[]> opponentThreatMap;
    public static List<int[]> generatedMoves;
    public static List<Piece> pieces = new List<Piece>();
    public static List<Piece> checkPieces = new List<Piece>();
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

    public static bool capturing = false;
    public static bool[] whiteCastlingRights; // Ks AND Qs
    public static bool[] blackCastlingRights; // Ks AND Qs
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
    public Piece castlingRook;
    public List<Piece> pieces;
    //public Dictionary<int[], Piece> board;

    public Move(Piece selectedPiece, int[] from, int[] to, bool moved, bool capturing, Piece capturedPiece, bool upgraded, Piece upgradedPiece, bool enPassant, int[] enPassantSquare, Piece enPassantPiece, bool castled, Piece castlingRook/*, Dictionary<int[], Piece> board*/, List<Piece> pieces)
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
