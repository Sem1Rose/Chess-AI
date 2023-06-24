using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MovesGenerator
{
    public static List<int[]> GenerateThreatMap(bool updateCheck = true, bool opponent = true)
    {
        int color = opponent ? (Board.turnToMove == ChessPieceTypes.White ? ChessPieceTypes.Black : ChessPieceTypes.White) : Board.turnToMove;

        bool white = color == ChessPieceTypes.White;

        List<int[]> generatedMoves = new List<int[]>();

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

            switch (Essentials.GetType(selectedPiece))
            {
                case ChessPieceTypes.Pawn:
                    // Capturing squares
                    if (bounds[(white ? 0 : 1)] > 0)
                    {
                        int[] pos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (white ? 1 : -1) };
                        for (int i = 0; i < 2; i++)
                        {
                            if (bounds[2 + i] > 0)
                            {
                                int[] move = Essentials.Translate(pos, i + 2);
                                generatedMoves.Add(move);

                                if (updateCheck && move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)))
                                    Board.checkPieces.Add(selectedPiece);
                            }
                        }
                    }
                    break;

                case ChessPieceTypes.king:
                    // Cross squares
                    for (int i = 0; i < 4; i++)
                    {
                        if (bounds[i] >= 1)
                        {
                            int[] move = Essentials.Translate(selectedPiece.position, i);
                            generatedMoves.Add(move);
                        }
                    }

                    // Diagonal squares
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (Mathf.Min(bounds[i], bounds[j + 2]) >= 1)
                            {
                                int[] move = Essentials.Translate(selectedPiece.position, 4 + j + i * 2);
                                generatedMoves.Add(move);
                            }
                        }
                    }
                    break;

                case ChessPieceTypes.Knight:
                    // Up and Down squares
                    for (int k = 0; k < 2; k++)
                    {
                        int[] startingPos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (k == 0 ? 2 : -2) };
                        for (int i = 0; i < 2; i++)
                        {
                            if (bounds[k] >= 2 && bounds[i + 2] >= 1)
                            {
                                int[] move = Essentials.Translate(startingPos, i + 2);

                                generatedMoves.Add(move);
                                if (updateCheck && move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)))
                                    Board.checkPieces.Add(selectedPiece);
                            }
                        }
                    }

                    // Right and Left squares
                    for (int k = 0; k < 2; k++)
                    {
                        int[] startingPos = new int[2] { selectedPiece.position[0] + (k == 0 ? 2 : -2), selectedPiece.position[1] };
                        for (int i = 0; i < 2; i++)
                        {
                            if (bounds[k + 2] >= 2 && bounds[i] >= 1)
                            {
                                int[] move = Essentials.Translate(startingPos, i);

                                generatedMoves.Add(move);
                                if (updateCheck && move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)))
                                    Board.checkPieces.Add(selectedPiece);
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
                                int[] move = Essentials.Translate(selectedPiece.position, 4 + j + i * 2, k);

                                if (Board.pieces.Any(x => x.position.SequenceEqual(move) && !Essentials.CheckType(x, ChessPieceTypes.king)))
                                {
                                    generatedMoves.Add(move);
                                    break;
                                }
                                else if (Board.pieces.Any(x => x.position.SequenceEqual(move) && Essentials.CheckType(x, ChessPieceTypes.king)))
                                {
                                    if (Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                    {
                                        generatedMoves.Add(move);
                                        break;
                                    }
                                }

                                generatedMoves.Add(move);
                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);
                            }
                        }
                    }
                    break;

                case ChessPieceTypes.Rook:
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 1; i <= bounds[j]; i++)
                        {
                            int[] move = Essentials.Translate(selectedPiece.position, j, i);

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move) && !Essentials.CheckType(x, ChessPieceTypes.king)))
                            {
                                generatedMoves.Add(move);
                                break;
                            }
                            else if (Board.pieces.Any(x => x.position.SequenceEqual(move) && Essentials.CheckType(x, ChessPieceTypes.king)))
                            {
                                if (Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                {
                                    generatedMoves.Add(move);
                                    break;
                                }
                            }

                            generatedMoves.Add(move);
                            if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                Board.checkPieces.Add(selectedPiece);
                        }
                    }
                    break;

                case ChessPieceTypes.Queen:
                    // Cross squares
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 1; i <= bounds[j]; i++)
                        {
                            int[] move = Essentials.Translate(selectedPiece.position, j, i);

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move) && !Essentials.CheckType(x, ChessPieceTypes.king)))
                            {
                                generatedMoves.Add(move);
                                break;
                            }
                            else if (Board.pieces.Any(x => x.position.SequenceEqual(move) && Essentials.CheckType(x, ChessPieceTypes.king)))
                            {
                                if (Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                {
                                    generatedMoves.Add(move);
                                    break;
                                }
                            }

                            generatedMoves.Add(move);
                            if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                Board.checkPieces.Add(selectedPiece);
                        }
                    }

                    // Diagonal squares
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            for (int k = 1; k <= MathF.Min(bounds[i], bounds[j + 2]); k++)
                            {
                                int[] move = Essentials.Translate(selectedPiece.position, 4 + j + i * 2, k);

                                if (Board.pieces.Any(x => x.position.SequenceEqual(move) && !Essentials.CheckType(x, ChessPieceTypes.king)))
                                {
                                    generatedMoves.Add(move);
                                    break;
                                }
                                else if (Board.pieces.Any(x => x.position.SequenceEqual(move) && Essentials.CheckType(x, ChessPieceTypes.king)))
                                {
                                    if (Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                    {
                                        generatedMoves.Add(move);
                                        break;
                                    }
                                }

                                generatedMoves.Add(move);
                                if (move.SequenceEqual((white ? Board.blackKing.position : Board.whiteKing.position)) && updateCheck)
                                    Board.checkPieces.Add(selectedPiece);
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
        int color = Essentials.GetColor(selectedPiece);
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
                // Normal movement
                for (int i = 1; i <= (selectedPiece.moved ? 1 : 2); i++)
                {
                    if ((white ? bounds[0] : bounds[1]) >= i)
                    {
                        int[] move = Essentials.Translate(selectedPiece.position, white ? 0 : 1, i);

                        if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            break;

                        generatedMoves.Add(move);
                    }
                }

                // Capturing and EnPassant checks
                int[] pos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (white ? 1 : -1) };
                if (bounds[(white ? 0 : 1)] > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int[] move = Essentials.Translate(pos, i + 2);

                        if (bounds[i + 2] > 0)
                        {
                            if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
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
                //Diagonal squares
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (Mathf.Min(bounds[i], bounds[j + 2]) > 0)
                        {
                            int[] move = Essentials.Translate(selectedPiece.position, 4 + j + i * 2);
                            if (Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                                continue;

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                    generatedMoves.Add(move);
                            }
                            else
                                generatedMoves.Add(move);
                        }
                    }
                }

                //Up and Down
                for (int i = 0; i < 2; i++)
                {
                    if (bounds[i] >= 1)
                    {
                        int[] move = Essentials.Translate(selectedPiece.position, i);
                        if (Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                            continue;

                        if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                        {
                            if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                generatedMoves.Add(move);
                        }
                        else
                            generatedMoves.Add(move);
                    }
                }

                //Right and Left + Castling checks
                for (int i = 2; i < 4; i++)
                {
                    if (bounds[i] >= 1)
                    {
                        int[] move = Essentials.Translate(selectedPiece.position, i);
                        if (Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                            continue;

                        if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                        {
                            if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                generatedMoves.Add(move);
                        }
                        else
                        {
                            generatedMoves.Add(move);

                            // CASTLING
                            if (!Essentials.InCheck() && !selectedPiece.moved && (white ? Board.whiteCastlingRights[i - 2] : Board.blackCastlingRights[i - 2]))
                            {
                                move = Essentials.Translate(selectedPiece.position, i, 2);
                                if (Board.opponentThreatMap.Any(x => x.SequenceEqual(move)))
                                    continue;

                                int[] rookPos = new int[2] { (i == 2 ? 7 : 0), (white ? 0 : 7) };
                                Piece castlingRook = Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(rookPos));
                                if (!Board.pieces.Any(x => x.position.SequenceEqual(move)) && castlingRook != null && !castlingRook.moved)
                                {
                                    if (i == 3)
                                    {
                                        if (!Board.opponentThreatMap.Any(x => x.SequenceEqual(Essentials.Translate(selectedPiece.position, i, 3))) && !Board.pieces.Any(x => x.position.SequenceEqual(Essentials.Translate(selectedPiece.position, i, 3))))
                                            generatedMoves.Add(move);
                                    }
                                    else
                                        generatedMoves.Add(move);
                                }
                            }
                        }
                    }
                }
                break;

            case ChessPieceTypes.Knight:
                // Up and Down squares
                for (int k = 0; k < 2; k++)
                {
                    int[] startingPos = new int[2] { selectedPiece.position[0], selectedPiece.position[1] + (k == 0 ? 2 : -2) };
                    for (int i = 0; i < 2; i++)
                    {
                        if (bounds[k] > 1 && bounds[i + 2] > 0)
                        {
                            int[] move = Essentials.Translate(startingPos, i + 2);

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                    generatedMoves.Add(move);
                            }
                            else
                                generatedMoves.Add(move);
                        }
                    }
                }

                // Right and Left squares
                for (int k = 0; k < 2; k++)
                {
                    int[] startingPos = new int[2] { selectedPiece.position[0] + (k == 0 ? 2 : -2), selectedPiece.position[1] };
                    for (int i = 0; i < 2; i++)
                    {
                        if (bounds[k + 2] > 1 && bounds[i] > 0)
                        {
                            int[] move = Essentials.Translate(startingPos, i);

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
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
                            int[] move = Essentials.Translate(selectedPiece.position, 4 + j + i * 2, k);

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                    generatedMoves.Add(move);

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
                        int[] move = Essentials.Translate(selectedPiece.position, j, i);

                        if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                        {
                            if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                generatedMoves.Add(move);

                            break;
                        }

                        generatedMoves.Add(move);
                    }
                }
                break;

            case ChessPieceTypes.Queen:
                // Cross squares
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 1; i <= bounds[j]; i++)
                    {
                        int[] move = Essentials.Translate(selectedPiece.position, j, i);

                        if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                        {
                            if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                generatedMoves.Add(move);

                            break;
                        }

                        generatedMoves.Add(move);
                    }
                }

                // Diagonal squares
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 1; k <= MathF.Min(bounds[i], bounds[j + 2]); k++)
                        {
                            int[] move = Essentials.Translate(selectedPiece.position, 4 + j + i * 2, k);

                            if (Board.pieces.Any(x => x.position.SequenceEqual(move)))
                            {
                                if (!Essentials.CheckColor(Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(move)), color))
                                    generatedMoves.Add(move);

                                break;
                            }

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
            var selPiece = selectedPiece;
            Board.capturing = Board.pieces.Any(x => x.position.SequenceEqual(pseudoLegalMoves[i]) && !Essentials.CheckColor(x, Board.turnToMove));
            Board.capturedPiece = Board.capturing ? Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(pseudoLegalMoves[i])) : null;

            Move move = MovingHandler.MakeMove(selPiece, pseudoLegalMoves[i], false);

            if (!Essentials.InCheck(true))
                legalMoves.Add(pseudoLegalMoves[i]);

            MovingHandler.UndoMove(move, false);
        }

        return legalMoves;
    }

    public static List<int[][]> GenerateAllPseudoLegalMoves(bool opponent = false)
    {
        int color = opponent ? (Board.turnToMove == ChessPieceTypes.White ? ChessPieceTypes.Black : ChessPieceTypes.White) : Board.turnToMove;

        bool white = color == ChessPieceTypes.White;

        Piece[] availablePieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, color)).ToArray();
        List<int[][]> allMoves = new List<int[][]>();

        for (int l = 0; l < availablePieces.Length; l++)
            allMoves.Add(GeneratePseudoLegalMoves(availablePieces[l]).ToArray());

        return allMoves;
    }

    public static List<int[][]> GenerateAllLegalMoves(bool opponent = false)
    {
        int color = opponent ? (Board.turnToMove == ChessPieceTypes.White ? ChessPieceTypes.Black : ChessPieceTypes.White) : Board.turnToMove;

        bool white = color == ChessPieceTypes.White;

        Piece[] availablePieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, color)).ToArray();
        List<int[][]> allMoves = new List<int[][]>();

        for (int l = 0; l < availablePieces.Length; l++)
            allMoves.Add(GenerateLegalMoves(availablePieces[l]).ToArray());

        return allMoves;
    }
}