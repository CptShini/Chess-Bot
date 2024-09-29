using System;
using System.Threading;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5;

public class BestBotV5 : IChessBot
{
    private Thinker _thinker;

    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        _thinker.IterativeDeepening();
        
        ScoredMove currentBest = _thinker.CurrentBest;
        Console.WriteLine(currentBest);

        if (currentBest.Move == Move.NullMove) Thread.Sleep(timer.MillisecondsRemaining);
        return currentBest.Move;
    }
}