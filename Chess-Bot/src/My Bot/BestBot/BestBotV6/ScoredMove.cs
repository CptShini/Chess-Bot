using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

internal readonly struct ScoredMove
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
        
        IsCheckmate = Math.Abs(evaluation) > 9000;
    }
}