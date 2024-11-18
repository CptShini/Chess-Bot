using System;
using System.Threading;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

public class BestBotV6 : IChessBot
{
    /*
     * TODO:
     *
     * Better (forced) draw detection, just 1 repetition is enough for draw score?
     * Experiment with valueboards
     * Experiment with all constants
     * Re-add early-game randomness
     * Better King Safety
     *
     * Research 2nd video features (since everything up until now was basically video 1)
     */
    
    private Thinker _thinker;
    
    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        _thinker.IterativeDeepening();
        
        ScoredMove currentBest = _thinker.CurrentBest;
        Console.WriteLine($"BestBotV6: {currentBest}");

        if (currentBest.Move == Move.NullMove) Thread.Sleep(timer.MillisecondsRemaining * 2);
        return currentBest.Move;
    }
}