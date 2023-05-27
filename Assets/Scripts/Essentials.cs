using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Essentials
{
    static readonly int[,] directions = new int[8, 2] { 
        { 0, 1 },   //UP
        { 0, -1 },  //DOWN
        { 1, 0 },   //RIGHT
        { -1, 0 },  //LEFT
        { 1, 1 },   //UP-RIGHT
        { -1, 1 },  //UP-LEFT
        { 1, -1 },  //DOWN-RIGHT
        { -1, -1 }  //DOWN-LEFT
    };

    public static FENReading ReadFEN(string FEN)
    {
        List<Piece> pieces = new List<Piece>();

        string[] split = FEN.Split(' ');

        string[] positions = split[0].Split('/');

        for (int i = 0, rankN = 7; i < 8; i++, rankN--)
        {
            char[] rank = positions[i].ToCharArray();
            int fileN = 0;

            for (int j = 0; j < rank.Length; j++)
            {
                bool parsed = int.TryParse(rank[j].ToString(), out int skip);
                if (parsed)
                {
                    fileN += skip;

                    if (skip < 8)
                        continue;
                    else
                        break;
                }

                switch (rank[j])
                {
                    case 'p':
                        pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Pawn, new int[2]{fileN, rankN}));
                        fileN++;
                        break;
                    case 'r':
                        pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Rook, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'n':
                        pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Knight, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'b':
                        pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Bishop, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'q':
                        pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Queen, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'k':
                        pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.king, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'P':
                        pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Pawn, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'R':
                        pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Rook, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'N':
                        pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Knight, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'B':
                        pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Bishop, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'Q':
                        pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Queen, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    case 'K':
                        pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.king, new int[2] { fileN, rankN }));
                        fileN++;
                        break;
                    default:
                        break;
                }
            }
        }

        return new FENReading(pieces, split[1] == "w" ? 8 : 16, split[2], split[3] != "-" ? DecodePosition(split[3]) : null, int.Parse(split[4]), int.Parse(split[5]));
    }

    public static string GetPosition(int file, int rank) => ((char)(file + 65)).ToString().ToLower() + (rank + 1).ToString();
    public static int[] DecodePosition(string position)
    {
        int file, rank;
        Debug.Log(position);
        char[] pos = position.ToUpper().ToCharArray();
        
        file = pos[0] - 65;
        rank = int.Parse(pos[1].ToString()) - 1;

        return new int[2] { file, rank };
    }

    public static List<int[]> GenerateMoves(Piece selectedPiece)
    {
        bool white = CheckColor(selectedPiece, ChessPieceTypes.White);
        int[] bounds = new int[4]
        {
            7 - selectedPiece.position[1],  //UP
            selectedPiece.position[1],      //DOWN
            7 - selectedPiece.position[0],  //RIGHT
            selectedPiece.position[0]       //LEFT
        };

        List<int[]> generatedMoves = new List<int[]>();

        switch (GetType(selectedPiece))
        {
            case ChessPieceTypes.Pawn:
                for (int i = 1; i <= (selectedPiece.moved? 1 : 2); i++)
                {
                    if((white? bounds[0] : bounds[1]) >= i)
                    {
                        int[] move = Move(selectedPiece.position, white ? 0 : 1, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            break;

                        generatedMoves.Add(move);
                    }
                }
                
                int[] pos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (white ? 1 : -1) };
                for (int i = 0; i < 2; i++)
                {
                    int[] move = Move(pos, i + 2);
                    if (Board.board.Any(x => x.Key.SequenceEqual(move))) 
                    { 
                        if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                            generatedMoves.Add(move);
                    }
                    else if (Board.enPassantSquare != null && Board.enPassantSquare.SequenceEqual(move))
                    {
                        generatedMoves.Add(move);
                    }
                }
                break;

            case ChessPieceTypes.king:
                for (int i = 0; i < 2; i++) //UP and DOWN squares
                {
                    if (bounds[i] >= 1)
                    {
                        int[] move = Move(selectedPiece.position, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                        {
                            if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                generatedMoves.Add(move);
                        }
                        else
                            generatedMoves.Add(move);
                    }
                }
                for (int i = 2; i < 4; i++) //RIGHT and LEFT Squares
                {
                    if (bounds[i] >= 1)
                    {
                        int[] move = Move(selectedPiece.position, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                        {
                            if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                generatedMoves.Add(move);
                        }
                        else
                        {
                            generatedMoves.Add(move);

                            if(!selectedPiece.moved && bounds[i] >= 2 && selectedPiece.position[1] == (CheckColor(selectedPiece, ChessPieceTypes.White)? 0 : 7))
                            {
                                move = Move(selectedPiece.position, i, 2);
                                Piece castlingRook = Board.board.FirstOrDefault(x => x.Key.SequenceEqual(new int[2]{(i == 2? 7 : 0), (Essentials.CheckColor(Board.selectedPiece, ChessPieceTypes.White)? 0 : 7)})).Value;

                                if (!Board.board.Any(x => x.Key.SequenceEqual(move)) && castlingRook != null && !castlingRook.moved)
                                    generatedMoves.Add(move);    

                            }
                        }
                    }
                }
                for (int i = 0; i < 2; i++) //Diagonal Squares
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (Mathf.Min(bounds[i], bounds[j + 2]) >= 1)
                        {
                            int[] move = Move(selectedPiece.position, 4 + j + i * 2);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                    generatedMoves.Add(move);
                            }
                            else
                                generatedMoves.Add(move);
                        }
                    }
                }
                break;

            case ChessPieceTypes.Bishop:
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 1; k <= MathF.Min(bounds[i], bounds[j + 2]); k++)
                        {
                            int[] move = Move(selectedPiece.position, 4 + j + i * 2, k);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                {
                                    generatedMoves.Add(move);
                                }
                                break;
                            }

                            generatedMoves.Add(move);
                        }
                    }
                }
                break;

            case ChessPieceTypes.Rook:
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 1; i <= bounds[j]; i++)
                    {
                        int[] move = Move(selectedPiece.position, j, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                        {
                            if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                            {
                                generatedMoves.Add(move);
                            }
                            break;
                        }

                        generatedMoves.Add(move);
                    }
                }
                break;

            case ChessPieceTypes.Queen:
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 1; i <= bounds[j]; i++)
                    {
                        int[] move = Move(selectedPiece.position, j, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)  ))
                        {
                            if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                            {
                                generatedMoves.Add(move);
                            }
                            break;
                        }

                        generatedMoves.Add(move);
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 1; k <= MathF.Min(bounds[i], bounds[j + 2]); k++)
                        {
                            int[] move = Move(selectedPiece.position, 4 + j + i * 2, k);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                {
                                    generatedMoves.Add(move);
                                }
                                break;
                            }

                            generatedMoves.Add(move);
                        }
                    }
                }
                break;

            case ChessPieceTypes.Knight:
                int[] startingPos;
                for (int k = 0; k < 2; k++)
                {
                    startingPos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (k == 0 ? 2 : -2) };
                    for (int i = 0; i < 2; i++)
                    {
                        if (bounds[k] >= 2 && bounds[i + 2] >= 1)
                        {
                            int[] move = Move(startingPos, i + 2);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                {
                                    generatedMoves.Add(move);
                                }
                            }else
                                generatedMoves.Add(move);

                        }
                    }
                }
                for (int k = 0; k < 2; k++)
                {
                    startingPos = new int[2] { selectedPiece.position[0] + (k == 0 ? 2 : -2), selectedPiece.position[1] };
                    for (int i = 0; i < 2; i++)
                    {
                        if (bounds[k + 2] >= 2 && bounds[i] >= 1)
                        {
                            int[] move = Move(startingPos, i);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, GetColor(selectedPiece)))
                                {
                                    generatedMoves.Add(move);
                                }
                            }
                            else
                                generatedMoves.Add(move);
                        }
                    }
                }
                break;

            default:
                break;
        }
        return generatedMoves;
    }

    public static int GetType(Piece piece) => piece.type & 7;
    public static int GetColor(Piece piece) => piece.type & 24;
    
    public static bool CheckType(Piece piece, int type) => GetType(piece) == type;
    public static bool CheckColor(Piece piece, int color) => GetColor(piece) == color;

    public static int[] Move(int[] startingPos, int dir, int times = 1) => new int[2] { startingPos[0] + directions[dir, 0] * times, startingPos[1] + directions[dir, 1] * times };

    public static void ChangeTurn() => Board.turnToMove = Board.turnToMove == ChessPieceTypes.White ? ChessPieceTypes.Black : ChessPieceTypes.White;
}