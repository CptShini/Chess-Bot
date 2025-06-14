﻿using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning.Valueboards;

internal static class ValueboardEvaluator
{
    internal static int EvaluateValueboardMove(this Move move, bool isWhiteToMove, int enemyPieceCount)
    {
        int start = move.StartSquare.Index.FlipIndex(!isWhiteToMove);
        int target = move.TargetSquare.Index.FlipIndex(!isWhiteToMove);
        
        Valueboard valueboard = move.MovePieceType.GetValueboard();
        int startValue = valueboard[enemyPieceCount, start];
        int targetValue = valueboard[enemyPieceCount, target];

        return targetValue - startValue;
    }
    
    internal static int EvaluateValueboardPosition(this PieceType pieceType, int positionIndex, int enemyPieceCount)
    {
        Valueboard valueboard = pieceType.GetValueboard();
        return valueboard[enemyPieceCount, positionIndex];
    }
}