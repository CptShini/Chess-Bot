using System;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning;

internal static class MopUpEvaluator
{
    internal static int EvaluateMopUp(this Move move, Square enemyKing, int enemyPieceCount)
    {
        if (move.MovePieceType != PieceType.King) return 0;

        int startDist = ManhattanDistance(move.StartSquare, enemyKing);
        int targetDist = ManhattanDistance(move.TargetSquare, enemyKing);
        int distanceClosed = startDist - targetDist;
        int closenessScore = distanceClosed * MopUpClosenessFactor;
        
        float endgameFactor = 1f - (enemyPieceCount - 1) / 15f;
        return (int)(closenessScore * endgameFactor);
    }
    
    private static int ManhattanDistance(Square a, Square b) =>
        Math.Abs(a.File - b.File) + Math.Abs(a.Rank - b.Rank);
}