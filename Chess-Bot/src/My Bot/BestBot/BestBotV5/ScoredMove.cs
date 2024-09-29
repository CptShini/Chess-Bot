using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5;

internal readonly struct ScoredMove
{
    internal readonly Move Move;
    internal readonly bool IsCheckmate;
    
    private readonly int _evaluation;
    private readonly int _depth;
    
    internal ScoredMove(Move move, int evaluation, int depth)
    {
        Move = move;
        _evaluation = evaluation;
        _depth = depth;
        
        IsCheckmate = Math.Abs(evaluation) > 9000;
        if (!IsCheckmate) return;
        
        _depth = evaluation < 0 ? 2 - _depth : _depth - 1;
    }
    
    public override string ToString() => $"{Move} | {(IsCheckmate ? $"Mate in {_depth}" : $"Depth: {_depth} | Evaluation: {_evaluation}")}";
}