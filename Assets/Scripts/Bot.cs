using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    bool zeroMoves = false;

    void Update()
    {
        if (GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvB && Board.turnToMove != ChessPieceTypes.Black && !zeroMoves)
            return;
        else if (GraphicsHandler.handler.gameMode == GraphicsHandler.gameModes.PvP)
            return;

        Piece[] pieces = Board.pieces.FindAll(x => Essentials.CheckColor(x, ChessPieceTypes.Black)).ToArray();
        List<int[][]> moves = new List<int[][]>();

        int numMoves = 0;
        for (int i = 0; i < pieces.Length; i++)
        {
            List<int[]> pieceMoves = MovesGenerator.GenerateLegalMoves(pieces[i]);
            numMoves = Mathf.Max(numMoves, pieceMoves.Count);
            moves.Add(pieceMoves.ToArray());
        }

        if (numMoves == 0)
        {
            zeroMoves = true;
            return;
        }

        int[][][] movesA = moves.ToArray();
        int rand1 = Random.Range(0, movesA.GetLength(0));

        while (movesA[rand1].GetLength(0) == 0)
            rand1 = Random.Range(0, movesA.GetLength(0));

        int rand2 = Random.Range(0, movesA[rand1].GetLength(0));

        int[] boardMove = movesA[rand1][rand2];

        Board.capturing = Board.board.Any(x => x.Key.SequenceEqual(boardMove)) && Essentials.CheckColor(Board.board.FirstOrDefault(x => x.Key.SequenceEqual(boardMove)).Value, Board.turnToMove);
        Board.selectedPiece = pieces[rand1];
        Board.generatedMoves = MovesGenerator.GenerateLegalMoves(Board.selectedPiece);
        Board.selectedSquare = boardMove;
        Board.capturedPiece = Board.capturing ? Board.board.FirstOrDefault(x => x.Key.SequenceEqual(boardMove)).Value : null;

        Move move = MovingHandler.MakeMove(ref Board.selectedPiece, Board.selectedSquare, ref Board.enPassantPiece, ref Board.enPassantSquare, ref Board.capturing, ref Board.capturedPiece, ref Board.board, ref Board.pieces);
        if (move != null)
            Board.lastMove = move;
    }
}
