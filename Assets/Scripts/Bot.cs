namespace Chess
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public class Bot : MonoBehaviour
    {
        bool zeroMoves = false;

        void Update()
        {
            if (Board.gameEnded)
                return;
            if (Board.gameMode == GameModes.PvP)
                return;
            else if (Board.gameMode == GameModes.PvB && Board.turnToMove != ChessPieceTypes.Black && !zeroMoves)
                return;

            Piece[] availablePieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, Board.turnToMove)).ToArray();
            int[][][] allMoves = MovesGenerator.GenerateAllLegalMoves().ToArray();

            List<int[]> listMoves = new List<int[]>();
            for (int i = 0; i < allMoves.GetLength(0); i++)
            {
                for (int j = 0; j < allMoves[i].GetLength(0); j++)
                {
                    listMoves.Add(allMoves[i][j]);
                }
            }

            int numPieces = allMoves.GetLength(0);
            int randomPieceIndex = Random.Range(0, numPieces);
            while (allMoves[randomPieceIndex].GetLength(0) == 0)
                randomPieceIndex = Random.Range(0, numPieces);

            Piece selectedPiece = Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(availablePieces[randomPieceIndex].position));

            int numMoves = allMoves[randomPieceIndex].GetLength(0);
            int randomMoveIndex = Random.Range(0, numMoves);

            int[] selectedMove = allMoves[randomPieceIndex][randomMoveIndex];

            Board.selectedPiece = selectedPiece;
            Board.selectedSquare = selectedMove;
            Board.capturing = Board.pieces.Any(x => x.position.SequenceEqual(selectedMove) && !Essentials.CheckColor(x, Board.turnToMove));
            Board.capturedPiece = Board.capturing ? Board.pieces.FirstOrDefault(x => x.position.SequenceEqual(selectedMove)) : null;

            Board.generatedMoves = allMoves[randomPieceIndex].ToList();

            Board.lastMove = MovingHandler.MakeMove(Board.selectedPiece, Board.selectedSquare, true);
            GameHandler.handler.UpdateUI();

            GraphicsHandler.handler.ResetBoard(true);
            GraphicsHandler.handler.HighlightMove(Board.lastMove.from, Board.lastMove.to);
        }
    }
}