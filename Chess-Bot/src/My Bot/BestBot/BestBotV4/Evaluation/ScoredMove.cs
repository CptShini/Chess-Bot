using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal readonly struct ScoredMove
{
    internal Move Move => _line.Moves[0];
    internal readonly bool IsCheckmate;

    private readonly Line _line;
    internal readonly int Evaluation;
    internal readonly int Depth;
    
    internal ScoredMove(Line line, int evaluation)
    {
        _line = line;
        Evaluation = evaluation;
        Depth = line.Depth;
        
        IsCheckmate = Math.Abs(evaluation) > 9000;
    }
}