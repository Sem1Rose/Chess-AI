using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    bool zeroMoves = false;

    void Update()
    {
        if(GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvB && Board.turnToMove != ChessPieceTypes.Black && !zeroMoves)
            return;
        else if(GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvP)
            return;
        
        Piece[] pieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, ChessPieceTypes.Black)).ToArray();
        List<int[][]> moves = new List<int[][]>();

        int numMoves = 0;
        for (int i = 0; i < pieces.Length; i++)
        {
            List<int[]> pieceMoves = Essentials.GenerateMoves(pieces[i]);
            numMoves = Mathf.Max(numMoves, pieceMoves.Count);
            moves.Add(pieceMoves.ToArray());
        }

        int[][][] movesA = moves.ToArray();

        if(numMoves == 0)
        {
            zeroMoves = true;
            return;
        }

        int rand1 = Random.Range(0, movesA.GetLength(0));

        while(movesA[rand1].GetLength(0) == 0)
        {
            rand1 = Random.Range(0, movesA.GetLength(0));
        }

        int rand2 = Random.Range(0, movesA[rand1].GetLength(0));

        int[] move = movesA[rand1][rand2];

        Board.capturing = Board.board.Any(x => x.Key.SequenceEqual(move));
        Board.selectedPiece = pieces[rand1];
        Board.selectedSquare = move;
        Board.capturedPiece = Board.capturing? Board.board.FirstOrDefault(x => x.Key.SequenceEqual(move)).Value : null;

        MovingHandler.Move();
    }
}
