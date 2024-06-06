using System.Diagnostics;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Thinking;

internal class Thinker
{
    private const int DepthHardLimit = 64;
    private const int ExpectedTurnCount = 40;
    
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
        ScoredMove currentBest = Think(0, out long timeTaken);
        
        for (int depth = 1; depth < DepthHardLimit; depth++)
        {
            long thinkTimeEstimate = GetThinkTimeEstimate(timeTaken);
            if (TimeToStopThinking(thinkTimeEstimate)) break;

            currentBest = Think(depth, out timeTaken);
            if (currentBest.IsCheckmate) break;
        }
        
        return currentBest;
    }

    private static long GetThinkTimeEstimate(long previousThinkTime = 0) => previousThinkTime * 7 + 30;

    private bool TimeToStopThinking(long thinkTimeEstimate)
    {
        long estimatedEndTime = _timer.MillisecondsElapsedThisTurn + thinkTimeEstimate;
        return estimatedEndTime > _maximumThinkTime;
    }

    private ScoredMove Think(int maxDepth, out long timeTaken)
    {
        Stopwatch s = Stopwatch.StartNew();
        ScoredMove result = _searcher.SearchForBestMove(maxDepth);
        timeTaken = s.ElapsedMilliseconds;
        
        return result;
    }
}