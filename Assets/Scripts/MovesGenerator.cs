using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MovesGenerator
{
    public static HashSet<int[]> GenerateThreatMap(bool updateCheck = true, bool opponent = true)
    {
        int color = opponent ? Board.turnToMove == ChessPieceTypes.White ? ChessPieceTypes.Black : ChessPieceTypes.White : Board.turnToMove;

        bool white = color == ChessPieceTypes.White;

        HashSet<int[]> generatedMoves = new HashSet<int[]>();

        Piece[] availablePieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, color)).ToArray();
        if (updateCheck)
            Board.checkPieces.Clear();

        for (int l = 0; l < availablePieces.Length; l++)
        {
            Piece selectedPiece = availablePieces[l];

            int[] bounds = new int[4]
            {
                7 - selectedPiece.position[1],  //UP
                selectedPiece.position[1],      //DOWN
                7 - selectedPiece.position[0],  //RIGHT
                selectedPiece.position[0]       //LEFT
            };
            // Add a check to only skip the current blocking piece if it was different color in addition to being a king 
            switch (Essentials.GetType(selectedPiece))
            {
                case ChessPieceTypes.Pawn:
                    if (bounds[(white ? 0 : 1)] > 0)
                    {
                        int[] pos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (white ? 1 : -1) };

                        for (int i = 0; i < 2; i++)
                        {
                            if (bounds[2 + i] > 0)
                            {
                                int[] move = Essentials.Move(pos, i + 2);

                                if (!generatedMoves.Contains(move))
                                    generatedMoves.Add(move);

                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);
                            }
                        }
                    }
                    break;

                case ChessPieceTypes.king:
                    for (int i = 0; i < 2; i++) //UP and DOWN squares
                    {
                        if (bounds[i] >= 1)
                        {
                            int[] move = Essentials.Move(selectedPiece.position, i);

                            if (!generatedMoves.Contains(move))
                                generatedMoves.Add(move);
                        }
                    }
                    for (int i = 2; i < 4; i++) //RIGHT and LEFT Squares
                    {
                        if (bounds[i] >= 1)
                        {
                            int[] move = Essentials.Move(selectedPiece.position, i);

                            if (!generatedMoves.Contains(move))
                                generatedMoves.Add(move);

                        }
                    }
                    for (int i = 0; i < 2; i++) //Diagonals
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (Mathf.Min(bounds[i], bounds[j + 2]) >= 1)
                            {
                                int[] move = Essentials.Move(selectedPiece.position, 4 + j + i * 2);

                                if (!generatedMoves.Contains(move))
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
                                int[] move = Essentials.Move(selectedPiece.position, 4 + j + i * 2, k);

                                if (Board.board.Any(x => x.Key.SequenceEqual(move) && !Essentials.CheckType(x.Value, ChessPieceTypes.king)) && !generatedMoves.Contains(move))
                                {
                                    generatedMoves.Add(move);
                                    if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                        Board.checkPieces.Add(selectedPiece);

                                    break;
                                }

                                if (!generatedMoves.Contains(move))
                                {
                                    generatedMoves.Add(move);
                                    if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                        Board.checkPieces.Add(selectedPiece);
                                }
                            }
                        }
                    }
                    break;

                case ChessPieceTypes.Rook:
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 1; i <= bounds[j]; i++)
                        {
                            int[] move = Essentials.Move(selectedPiece.position, j, i);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move) && !Essentials.CheckType(x.Value, ChessPieceTypes.king)) && !generatedMoves.Contains(move))
                            {
                                generatedMoves.Add(move);
                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);

                                break;
                            }

                            if (!generatedMoves.Contains(move))
                            {
                                generatedMoves.Add(move);
                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);
                            }
                        }
                    }
                    break;

                case ChessPieceTypes.Queen:
                    for (int j = 0; j < 4; j++) // CROSS
                    {
                        for (int i = 1; i <= bounds[j]; i++)
                        {
                            int[] move = Essentials.Move(selectedPiece.position, j, i);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move) && !Essentials.CheckType(x.Value, ChessPieceTypes.king)) && !generatedMoves.Contains(move))
                            {
                                generatedMoves.Add(move);
                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);

                                break;
                            }

                            if (!generatedMoves.Contains(move))
                            {
                                generatedMoves.Add(move);
                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);
                            }
                        }
                    }
                    for (int i = 0; i < 2; i++) // DIAGONALS
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            for (int k = 1; k <= MathF.Min(bounds[i], bounds[j + 2]); k++)
                            {
                                int[] move = Essentials.Move(selectedPiece.position, 4 + j + i * 2, k);

                                if (Board.board.Any(x => x.Key.SequenceEqual(move) && !Essentials.CheckType(x.Value, ChessPieceTypes.king)) && !generatedMoves.Contains(move))
                                {
                                    generatedMoves.Add(move);
                                    if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                        Board.checkPieces.Add(selectedPiece);

                                    break;
                                }

                                if (!generatedMoves.Contains(move))
                                {
                                    generatedMoves.Add(move);
                                    if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                        Board.checkPieces.Add(selectedPiece);
                                }
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
                                int[] move = Essentials.Move(startingPos, i + 2);

                                if (!generatedMoves.Contains(move))
                                {
                                    generatedMoves.Add(move);
                                    if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                        Board.checkPieces.Add(selectedPiece);
                                }
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
                                int[] move = Essentials.Move(startingPos, i);

                                if (!generatedMoves.Contains(move))
                                {
                                    generatedMoves.Add(move);
                                    if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                        Board.checkPieces.Add(selectedPiece);
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }
        return generatedMoves;
    }

    public static List<int[]> GeneratePseudoLegalMoves(Piece selectedPiece)
    {
        bool white = Essentials.CheckColor(selectedPiece, ChessPieceTypes.White);

        int[] bounds = new int[4]
        {
            7 - selectedPiece.position[1],  //UP
            selectedPiece.position[1],      //DOWN
            7 - selectedPiece.position[0],  //RIGHT
            selectedPiece.position[0]       //LEFT
        };

        List<int[]> generatedMoves = new List<int[]>();

        switch (Essentials.GetType(selectedPiece))
        {
            case ChessPieceTypes.Pawn:
                for (int i = 1; i <= (selectedPiece.moved ? 1 : 2); i++)
                {
                    if ((white ? bounds[0] : bounds[1]) >= i)
                    {
                        int[] move = Essentials.Move(selectedPiece.position, white ? 0 : 1, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            break;

                        generatedMoves.Add(move);
                    }
                }

                int[] pos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (white ? 1 : -1) };
                if (bounds[(white ? 0 : 1)] > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int[] move = Essentials.Move(pos, i + 2);

                        if (bounds[2 + i] > 0)
                        {
                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
                                    generatedMoves.Add(move);
                            }
                            else if (Board.enPassantSquare != null && Board.enPassantSquare.SequenceEqual(move))
                            {
                                generatedMoves.Add(move);
                            }
                        }
                    }
                }
                break;

            case ChessPieceTypes.king:
                for (int i = 0; i < 2; i++) //UP and DOWN squares
                {
                    if (bounds[i] >= 1)
                    {
                        int[] move = Essentials.Move(selectedPiece.position, i);
                        if (!Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                        {
                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
                                    generatedMoves.Add(move);
                            }
                            else
                                generatedMoves.Add(move);
                        }
                        else
                            Debug.Log(move[0] + " " + move[1]);
                    }
                }
                for (int i = 2; i < 4; i++) //RIGHT and LEFT Squares
                {
                    if (bounds[i] >= 1)
                    {
                        int[] move = Essentials.Move(selectedPiece.position, i);
                        if (!Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                        {
                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
                                    generatedMoves.Add(move);
                            }
                            else
                            {
                                generatedMoves.Add(move);

                                // CASTLING
                                if (!Essentials.InCheck() && !selectedPiece.moved && (white ? Board.whiteCastlingRights[i - 2] : Board.blackCastlingRights[i - 2]))
                                {
                                    move = Essentials.Move(selectedPiece.position, i, 2);

                                    if (!Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                                    {
                                        Piece castlingRook = Board.board.FirstOrDefault(x => x.Key.SequenceEqual(new int[2] { (i == 2 ? 7 : 0), (Essentials.CheckColor(Board.selectedPiece, ChessPieceTypes.White) ? 0 : 7) })).Value;
                                        if (!Board.board.Any(x => x.Key.SequenceEqual(move)) && castlingRook != null && !castlingRook.moved)
                                        {
                                            if (i == 3)
                                            {
                                                if (!Board.opponentThreatMap.Contains(Essentials.Move(move, i)) && !Board.board.Any(x => x.Key.SequenceEqual(Essentials.Move(move, i))))
                                                    generatedMoves.Add(move);
                                            }
                                            else
                                                generatedMoves.Add(move);
                                        }
                                    }
                                }
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
                            int[] move = Essentials.Move(selectedPiece.position, 4 + j + i * 2);
                            if (!Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                            {
                                if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                                {
                                    if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
                                        generatedMoves.Add(move);
                                }
                                else
                                    generatedMoves.Add(move);
                            }
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
                            int[] move = Essentials.Move(selectedPiece.position, 4 + j + i * 2, k);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
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
                        int[] move = Essentials.Move(selectedPiece.position, j, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                        {
                            if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
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
                        int[] move = Essentials.Move(selectedPiece.position, j, i);

                        if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                        {
                            if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
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
                            int[] move = Essentials.Move(selectedPiece.position, 4 + j + i * 2, k);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
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
                            int[] move = Essentials.Move(startingPos, i + 2);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
                                {
                                    generatedMoves.Add(move);
                                }
                            }
                            else
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
                            int[] move = Essentials.Move(startingPos, i);

                            if (Board.board.Any(x => x.Key.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckType(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value, Essentials.GetColor(selectedPiece)))
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

    public static List<int[]> GenerateLegalMoves(Piece selectedPiece)
    {
        List<int[]> pseudoLegalMoves = GeneratePseudoLegalMoves(selectedPiece);
        List<int[]> legalMoves = new List<int[]>();

        for (int i = 0; i < pseudoLegalMoves.Count; i++)
        {
            var temp = selectedPiece;
            Move move = MovingHandler.MakeMove(ref temp, pseudoLegalMoves[i], ref Board.enPassantPiece, ref Board.enPassantSquare, ref Board.capturing, ref Board.capturedPiece, ref Board.board, ref Board.pieces, false, pseudoLegalMoves);

            if (!Essentials.InCheck(false))
                legalMoves.Add(pseudoLegalMoves[i]);

            MovingHandler.ApplyMove(MovingHandler.UndoMove(move, false));
        }

        return legalMoves;
    }
}