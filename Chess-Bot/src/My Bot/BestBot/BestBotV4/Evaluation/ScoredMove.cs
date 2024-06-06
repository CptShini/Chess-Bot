using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal readonly struct ScoredMove : IComparable<ScoredMove>
{
    private static readonly Random Random = new();
    
    internal readonly Move Move;
    internal readonly bool IsCheckmate;
    
    private readonly int _evaluation;
    private readonly int _depth;
    
    internal ScoredMove(Move move, int evaluation, int depth)
    {
        Move = move;
        _evaluation = evaluation;
        _depth = depth;

        int absEval = Math.Abs(evaluation);
        IsCheckmate = absEval > 9000;
        if (!IsCheckmate) return;
        
        _depth = 10000 - absEval;
        _depth = evaluation < 0 ? -_depth : _depth;
    }

    public int CompareTo(ScoredMove other)
    {
        int evaluationComparator = _evaluation.CompareTo(other._evaluation);
        bool sameEvaluation = evaluationComparator == 0;
        if (!sameEvaluation) return evaluationComparator;

        int depthComparator = other._depth.CompareTo(_depth);
        bool sameDepth = depthComparator == 0;
        if (!sameDepth) return depthComparator;
        
        bool isFunnier = Random.Next(2) == 0;
        return isFunnier ? 1 : 0;
    }
    
    public override string ToString() => $"{Move} | {(IsCheckmate ? $"Mate in {_depth}" : $"Depth: {_depth} | Evaluation: {_evaluation}")}";
}