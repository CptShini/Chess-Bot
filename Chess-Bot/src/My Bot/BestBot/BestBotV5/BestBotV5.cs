using System;
using System.Threading;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5;

public class BestBotV5 : IChessBot
{
    /*
    TODO:
        PVS
        Iterative Deepening
            (PVS) Move Ordering
            Aspiration Windows
        Transposition Table
    */

    private Thinker _thinker;

    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        
        ScoredMove scoredMove = _thinker.IterativeDeepening();
        Console.WriteLine(scoredMove);
        
        if (scoredMove.Move == Move.NullMove) Thread.Sleep(timer.MillisecondsRemaining);
        return scoredMove.Move;
    }
}