using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4;

public class BestBotV4 : IChessBot
{
    /*
    TODO:
        Endgame play, (How to evaluate?):
            King to King distance
            Enemy king near edges
        Transposition Table
        Move Ordering from Iterative Deepening
    */

    private Thinker _thinker;

    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        
        ScoredMove scoredMove = _thinker.IterativeDeepening();
        Console.WriteLine(scoredMove);
        
        return scoredMove.Move;
    }
}