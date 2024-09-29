using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3;

public class BestBotV3 : IChessBot
{
    /*
    TODO:
        Endgame play, (How to evaluate?):
            King to King distance
            Enemy king near edges
        Transposition Table
        Omega high depth/repetition bug?
    */

    private Thinker _thinker;

    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        
        ScoredMove scoredMove = _thinker.IterativeDeepening();
        Console.WriteLine($"BestBotV3: {scoredMove}");
        
        return scoredMove.Move;
    }
}