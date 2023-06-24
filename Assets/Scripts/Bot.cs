using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Bot : MonoBehaviour
{
    bool zeroMoves = false;

    IEnumerator Movement(float f)
    {
        while (true)
        {
            if (GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvP)
                yield break;
            else if (GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvB && Board.turnToMove != ChessPieceTypes.Black && !zeroMoves)
                yield break;

            yield return new WaitForSeconds(f);

            Piece[] availablePieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, Board.turnToMove)).ToArray();
            int[][][] allMoves = MovesGenerator.GenerateAllLegalMoves().ToArray();

            int maxNumMoves = 0;
            List<int[]> listMoves = new List<int[]>();
            for (int i = 0; i < allMoves.GetLength(0); i++)
            {
                if (allMoves[i].GetLength(0) > maxNumMoves)
                    maxNumMoves = allMoves[i].GetLength(0);
                for (int j = 0; j < allMoves[i].GetLength(0); j++)
                {
                    listMoves.Add(allMoves[i][j]);
                }
            }

            if (maxNumMoves == 0)
            {
                zeroMoves = true;
                yield break;
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
            Debug.Log("bot");

            Move move = MovingHandler.MakeMove(Board.selectedPiece, Board.selectedSquare, true);
        }
    }

    void Start()
    {
        var coroutine = Movement(.25f);
        StartCoroutine(coroutine);
    }
}
