using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;
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
     * King Safety
     *
     */
    
    private Thinker _thinker;
    
    public Move Think(Board board, Timer timer)
    {
        Dictionary<Move, int> scoredMoves = new();
        foreach (Move move in board.GetLegalMoves())
        {
            int eval = PositionEvaluator.EvaluateMovePositioning(move, board);
            scoredMoves.Add(move, eval);
        }

        foreach ((Move move, int eval) in scoredMoves.OrderByDescending(kvp => kvp.Value))
        {
            Console.WriteLine($"{move} | Eval: {eval}");
        }

        _thinker = new(board, timer);
        _thinker.IterativeDeepening();
        
        ScoredMove currentBest = _thinker.CurrentBest;
        Console.WriteLine($"BestBotV6: {currentBest}");

        if (currentBest.Move == Move.NullMove) Thread.Sleep(timer.MillisecondsRemaining * 2);
        return currentBest.Move;
    }
}