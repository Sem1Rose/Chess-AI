using System.Collections.Generic;
using UnityEngine;

public static class Board 
{
    public static Dictionary<int[], Piece> board = new Dictionary<int[], Piece>();
    public static List<Piece> pieces = new List<Piece>();

    public static Piece selectedPiece = null;
    public static Piece capturedPiece = null;
    public static Piece enPassantPiece = null;

    public static int[] selectedSquare = null;
    public static int[] enPassantSquare = null;
    public static int turnToMove = ChessPieceTypes.White;
    public static bool capturing = false;
}


[System.Serializable]
public class Piece
{
    public int type;
    public int[] position;
    public bool moved;
    public GameObject gameobject;

    public Piece(int type, int[] position, GameObject gameobject, bool moved = false)
    {
        this.type = type;
        this.position = position;
        this.gameobject = gameobject;
        this.moved = moved;
    }
    public Piece(int type, int[] position, bool moved = false)
    {
        this.type = type;
        this.position = position;
        this.moved = moved;
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
    public string castling;
    public int[] enPassantTS;
    public int halfmoveC;
    public int fullmoveC;

    public FENReading(List<Piece> pieces, int turn, string castling, int[] enPassantTS, int halfmoveC, int fullmoveC)
    {
        this.pieces = pieces;
        this.turn = turn;
        this.castling = castling;
        this.enPassantTS = enPassantTS;
        this.halfmoveC = halfmoveC;
        this.fullmoveC = fullmoveC;
    }
}