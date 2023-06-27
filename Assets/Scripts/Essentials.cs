namespace Chess
{
    using System.Collections.Generic;

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
                            pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Pawn, new int[2] { fileN, rankN }, rankN == 6 ? false : true));
                            fileN++;
                            break;
                        case 'k':
                            pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.king, new int[2] { fileN, rankN }, (rankN == 7 || fileN == 4) ? false : true));
                            fileN++;
                            break;
                        case 'r':
                            pieces.Add(new Piece(ChessPieceTypes.Black | ChessPieceTypes.Rook, new int[2] { fileN, rankN }, (rankN == 7 || (fileN == 0 || fileN == 7)) ? false : true));
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
                        case 'P':
                            pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Pawn, new int[2] { fileN, rankN }, (rankN == 1) ? false : true));
                            fileN++;
                            break;
                        case 'K':
                            pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.king, new int[2] { fileN, rankN }, (rankN == 0 || fileN == 4) ? false : true));
                            fileN++;
                            break;
                        case 'R':
                            pieces.Add(new Piece(ChessPieceTypes.White | ChessPieceTypes.Rook, new int[2] { fileN, rankN }, (rankN == 0 || (fileN == 0 || fileN == 7)) ? false : true));
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

                        default:
                            break;
                    }
                }
            }

            bool[] whiteCastlingRights = new bool[2] { false, false };
            bool[] blackCastlingRights = new bool[2] { false, false };

            char[] castling = split[2].ToCharArray();
            for (int i = 0; i < castling.Length; i++)
            {
                switch (castling[i])
                {
                    case 'K':
                        whiteCastlingRights[0] = true;
                        break;
                    case 'Q':
                        whiteCastlingRights[1] = true;
                        break;
                    case 'k':
                        blackCastlingRights[0] = true;
                        break;
                    case 'q':
                        blackCastlingRights[1] = true;
                        break;

                    default:
                        break;
                }
            }

            int[] enPassantTS = null;
            if (split[3] != "-")
                enPassantTS = DecodePosition(split[3]);

            return new FENReading(pieces, split[1] == "w" ? ChessPieceTypes.White : ChessPieceTypes.Black, whiteCastlingRights, blackCastlingRights, enPassantTS, int.Parse(split[4]), int.Parse(split[5]));
        }

        public static string GetTurnToMove() => Board.turnToMove == ChessPieceTypes.White ? "White" : "Black";
        public static string GetPosition(int[] pos) => ((char)(pos[0] + 65)).ToString().ToLower() + (pos[1] + 1).ToString();
        public static string GenerateLastMoveLAN()
        {
            if (Board.lastMove == null)
                return "";

            string generatedMove = GetPieceLetter(Board.lastMove.selectedPiece);
            if (Board.lastMove.upgraded)
                generatedMove = "";

            generatedMove += GetPosition(Board.lastMove.from);

            if (Board.lastMove.capturing)
                generatedMove += "x";

            generatedMove += GetPosition(Board.lastMove.to);

            if (Board.lastMove.enPassant)
                generatedMove += " e.p.";
            else if (Board.lastMove.upgraded)
                generatedMove += "=" + GetPieceLetter(Board.lastMove.selectedPiece);
            else if (Board.lastMove.castled)
                generatedMove = Board.lastMove.kingSide ? "O-O" : "O-O-O";

            if (Board.gameEnded && Board.gameEndReason != Board.GameEndReason.Draw && Board.gameEndReason != Board.GameEndReason.StaleMate)
                generatedMove += "#";
            else if (InCheck())
                generatedMove += Board.numCheckers == 1 ? "+" : "++";

            return generatedMove;
        }
        public static string GetPieceLetter(Piece piece)
        {
            switch (GetType(Board.lastMove.selectedPiece))
            {
                case ChessPieceTypes.king:
                    return "K";
                case ChessPieceTypes.Queen:
                    return "Q";
                case ChessPieceTypes.Rook:
                    return "R";
                case ChessPieceTypes.Bishop:
                    return "B";
                case ChessPieceTypes.Knight:
                    return "N";
                default:
                    return "";
            }
        }


        public static int GetType(Piece piece) => piece.type & 7;
        public static int GetColor(Piece piece) => piece.type & 24;
        public static int OppositeColor(int color) => color == ChessPieceTypes.White ? ChessPieceTypes.Black : ChessPieceTypes.White;

        public static bool InCheck(bool opponent = false) { MovesGenerator.GenerateThreatMap(!opponent); return Board.numCheckers > 0; }
        public static bool CheckForMate()
        {
            int[][][] availableMoves = MovesGenerator.GenerateAllLegalMoves().ToArray();
            int maxNumMoves = 0;
            for (int i = 0; i < availableMoves.GetLength(0); i++)
            {
                if (availableMoves[i].GetLength(0) > maxNumMoves)
                    maxNumMoves = availableMoves[i].GetLength(0);
            }
            if (maxNumMoves == 0)
                return true;

            return false;
        }
        public static bool CheckType(Piece piece, int type) => GetType(piece) == type;
        public static bool CheckColor(Piece piece, int color) => GetColor(piece) == color;

        public static int[] Translate(int[] startingPos, int dir, int times = 1) => new int[2] { startingPos[0] + directions[dir, 0] * times, startingPos[1] + directions[dir, 1] * times };
        public static int[] DecodePosition(string position)
        {
            int file, rank;
            char[] pos = position.ToUpper().ToCharArray();

            file = pos[0] - 65;
            rank = int.Parse(pos[1].ToString()) - 1;

            return new int[2] { file, rank };
        }

        public static void ChangeTurn()
        {
            Board.turnToMove = OppositeColor(Board.turnToMove);
            Board.opponentThreatMap = MovesGenerator.GenerateThreatMap();
        }
    }
}