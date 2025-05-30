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
     * Sit the fuck down and actually learn how your own bot works
     * 
     * Fix search going infinitely when draw
     * Weighted average for TTE to prioritize more recent times
     * Better (forced) draw detection, just 1 repetition is enough for draw score?
     * Experiment with valueboards
     * Experiment with all constants
     * Better King Safety
     * Optimization check (try profiler)
     *  - Stop using hamming weight, just increment on enemy piece taken?
     *  - Faster move ordering
     * Retry Aspiration windows
     * Reexamine move ordering with iterative deepening and transposition tables
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