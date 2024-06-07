using System.Diagnostics;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Thinking;

internal class Thinker
{
    private const int Infinity = 999999;
    
    private const int DepthHardLimit = 64;
    private const int ExpectedTurnCount = 30;
    
    private readonly long _maximumThinkTime;
    private readonly Searcher _searcher;
    private readonly Timer _timer;

    internal Thinker(Board board, Timer timer)
    {
        _searcher = new(board);
        _timer = timer;
        
        _maximumThinkTime = timer.MillisecondsRemaining / ExpectedTurnCount;
    }

    internal ScoredMove IterativeDeepening()
    {
        int alpha = -Infinity;
        int beta = Infinity;
        
        ScoredMove currentBest = Think(0, alpha, beta, out long timeTaken);
        for (int depth = 1; depth < DepthHardLimit; depth++)
        {
            long thinkTimeEstimate = (long)GetThinkTimeEstimate(depth, timeTaken);
            if (TimeToStopThinking(thinkTimeEstimate)) break;

            currentBest = Think(depth, alpha, beta, out timeTaken);
            if (currentBest.IsCheckmate) break;
        }
        
        return currentBest;
    }
    
    private ScoredMove Think(int maxDepth, int alpha, int beta, out long timeTaken)
    {
        Stopwatch s = Stopwatch.StartNew();
        
        Line line = new();
        int evaluation = _searcher.Search(ref line, maxDepth, alpha, beta);
        ScoredMove result = new(line, evaluation);
        
        timeTaken = s.ElapsedMilliseconds;
        
        return result;
    }
    
    private bool TimeToStopThinking(long thinkTimeEstimate)
    {
        long estimatedEndTime = _timer.MillisecondsElapsedThisTurn + thinkTimeEstimate;
        return estimatedEndTime > _maximumThinkTime;
    }
    
    private static float GetThinkTimeEstimate(int depth, long previousThinkTime = 0)
    {
        return depth switch
        {
            0 => previousThinkTime + 10f,
            1 => previousThinkTime * 0.9902f + 0.0196f,
            2 => previousThinkTime * 0.7300f + 5.5400f,
            3 => previousThinkTime * 0.9394f + 12.532f,
            4 => previousThinkTime * 4.3539f + 19.885f,
            5 => previousThinkTime * 7.5979f + 23.850f,
            6 => previousThinkTime * 6.2155f + 414.05f,
            7 => previousThinkTime * 6.7331f - 91.745f,
            _ => previousThinkTime * 7f
        };
    }
}