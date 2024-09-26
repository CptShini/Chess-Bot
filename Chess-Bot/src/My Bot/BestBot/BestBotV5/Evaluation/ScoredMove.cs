using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal readonly struct ScoredMove
{
    internal Move Move => Line.Moves[0];
    internal readonly bool IsCheckmate;

    internal readonly Line Line;
    private readonly int _evaluation;
    private readonly int _depth;
    
    internal ScoredMove(Line line, int evaluation)
    {
        Line = line;
        _evaluation = evaluation;
        _depth = line.Depth;
        
        IsCheckmate = Math.Abs(evaluation) > 9000;
        if (!IsCheckmate) return;
        
        _depth = evaluation < 0 ? -_depth : _depth;
    }
    
    public override string ToString() => $"{Move} | {(IsCheckmate ? $"Mate in {_depth}" : $"Depth: {_depth} | Evaluation: {_evaluation}")}";
}