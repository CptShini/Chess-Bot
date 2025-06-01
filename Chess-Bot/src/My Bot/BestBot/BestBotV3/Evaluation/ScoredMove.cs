using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;

internal readonly struct ScoredMove : IComparable<ScoredMove>
{
    internal readonly Move Move;
    internal readonly bool IsCheckmate;
    
    internal readonly int Evaluation;
    internal readonly int Depth;
    
    internal ScoredMove(Move move, int evaluation, int depth)
    {
        Move = move;
        Evaluation = evaluation;
        Depth = depth;

        int absEval = Math.Abs(evaluation);
        IsCheckmate = absEval > 9000;
        if (!IsCheckmate) return;
        
        Depth = 10000 - absEval;
    }

    public int CompareTo(ScoredMove other)
    {
        int evaluationComparator = Evaluation.CompareTo(other.Evaluation);
        bool sameEvaluation = evaluationComparator == 0;
        if (!sameEvaluation) return evaluationComparator;

        int depthComparator = other.Depth.CompareTo(Depth);
        bool sameDepth = depthComparator == 0;
        if (!sameDepth) return depthComparator;
        
        bool isFunnier = Random.Next(2) == 0;
        return isFunnier ? 1 : 0;
    }
}