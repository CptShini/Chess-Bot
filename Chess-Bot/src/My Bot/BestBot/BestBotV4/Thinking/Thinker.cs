using System;
using System.Diagnostics;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Thinking;

internal class Thinker
{
    private const int Infinity = 999999;
    
    private const int DepthHardLimit = 32;
    private const int ExpectedTurnCount = 30;
    
    private readonly long _maximumThinkTime;
    private readonly Searcher _searcher;
    private readonly Stopwatch _timer;

    internal Thinker(Board board, Timer timer)
    {
        _searcher = new(board);
        _timer = Stopwatch.StartNew();
        
        _maximumThinkTime = timer.MillisecondsRemaining / ExpectedTurnCount * Stopwatch.Frequency / 1000;
    }

    internal ScoredMove IterativeDeepening()
    {
        ScoredMove currentBest = Think(out long timeTaken);
        for (int depth = 1; depth < DepthHardLimit; depth++)
        {
            long thinkTimeEstimate = GetThinkTimeEstimate(depth, timeTaken);
            if (TimeToStopThinking(thinkTimeEstimate)) break;

            currentBest = Think(out timeTaken, depth);
            if (currentBest.IsCheckmate) break;
        }
        
        return currentBest;
    }
    
    private ScoredMove Think(out long timeTaken, int maxDepth = 0, int alpha = -Infinity, int beta = Infinity)
    {
        Stopwatch s = Stopwatch.StartNew();
        
        Line line = new();
        int evaluation = _searcher.Search(ref line, maxDepth, alpha, beta);
        ScoredMove thoughtProduct = new(line, evaluation);
        
        timeTaken = s.ElapsedTicks;
        return thoughtProduct;
    }
    
    private bool TimeToStopThinking(long thinkTimeEstimate)
    {
        long estimatedEndTime = _timer.ElapsedTicks + thinkTimeEstimate;
        return estimatedEndTime > _maximumThinkTime;
    }
    
    private static long GetThinkTimeEstimate(int depth, long previousThinkTime = 0)
    {
        return (long)MathF.Max(0, EstimateThinkTime());

        float EstimateThinkTime()
        {
            return depth switch
            {
                0 => previousThinkTime + 225f,
                1 => previousThinkTime * 3.7755f + 323.92f,
                2 => previousThinkTime * 14.321f - 1762.3f,
                3 => previousThinkTime * 8.6170f - 6272.1f,
                4 => previousThinkTime * 11.293f - 102303f,
                5 => previousThinkTime * 5.2390f + 232060f,
                6 => previousThinkTime * 7.4013f + 972526f,
                7 => previousThinkTime * 3.9426f + 20000000f,
                _ => previousThinkTime * 7f
            };
        }
    }
}