using System;
using System.Diagnostics;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Thinking;

internal class Thinker
{
    private const int Infinity = 999999;
    
    private const int DepthHardLimit = 100;
    private const int ExpectedTurnCount = 40;
    
    private readonly long _maximumThinkTime;
    private static ThinkTimeEstimator _thinkTimeEstimator;
    private readonly Searcher _searcher;
    private readonly Stopwatch _timer;

    internal Thinker(Board board, Timer timer)
    {
        if (timer.GameStartTimeMilliseconds - timer.MillisecondsRemaining < 25) _thinkTimeEstimator = new();
        
        _searcher = new(board);
        _timer = Stopwatch.StartNew();

        float thinkTimeMilliseconds = (float)timer.MillisecondsRemaining / ExpectedTurnCount;
        _maximumThinkTime = (long)(thinkTimeMilliseconds * Stopwatch.Frequency / 1000);
    }

    internal ScoredMove IterativeDeepening()
    {
        ScoredMove currentBest = Think(out long timeTaken);
        for (int depth = 1; depth < DepthHardLimit; depth++)
        {
            long previousThinkTime = timeTaken;
            long thinkTimeEstimate = GetThinkTimeEstimate(depth, previousThinkTime);
            if (TimeToStopThinking(thinkTimeEstimate)) break;
            
            currentBest = Think(out timeTaken, depth);
            long currentThinkTime = timeTaken;
            
            float branchFactor = (float)currentThinkTime / previousThinkTime;
            _thinkTimeEstimator.AddBranch(depth, branchFactor);
            
            if (currentBest.IsCheckmate) break;
        }
        
        return currentBest;
    }
    
    private ScoredMove Think(out long timeTaken, int maxDepth = 0, int alpha = -Infinity, int beta = Infinity)
    {
        Stopwatch s = Stopwatch.StartNew();
        
        int evaluation = _searcher.Search(maxDepth);
        ScoredMove thoughtProduct = new(_searcher.BestMove, evaluation, maxDepth);
        
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
        float branchFactor = _thinkTimeEstimator.GetAverageBranchFactor(depth);
        if (branchFactor == 0f) branchFactor = 7f;
            
        return (long)(previousThinkTime * branchFactor);
    }
}