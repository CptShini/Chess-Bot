using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Thinking;

internal class Thinker
{
    private const int ExpectedTurnCount = 40;
    private const int MinimumTurnCount = 100;
    private const int SpeedUpTime = 2;
    private const int ComplexityCoefficient = 8;
    
    private readonly long _minimumThinkTime;
    private readonly long _maximumThinkTime;
    
    private readonly Searcher _searcher;
    private readonly Timer _timer;

    internal Thinker(Board board, Timer timer)
    {
        _searcher = new(board);
        _timer = timer;
        
        _minimumThinkTime = timer.GameStartTimeMilliseconds / MinimumTurnCount;
        
        int turnCountTime = timer.GameStartTimeMilliseconds / ExpectedTurnCount;
        int emergencyTime = (int)(timer.MillisecondsRemaining / (ExpectedTurnCount / (float)SpeedUpTime));
        _maximumThinkTime = Math.Min(turnCountTime, emergencyTime);
    }

    internal ScoredMove IterativeDeepening()
    {
        ScoredMove currentBest = Think(0, GetThinkTimeEstimate(), out long timeTaken);
        
        for (int depth = 1; ; depth++)
        {
            long thinkTimeEstimate = GetThinkTimeEstimate(timeTaken);
            if (EstimatedTimeTooSlow(thinkTimeEstimate)) break;

            try
            {
                currentBest = Think(depth, thinkTimeEstimate, out timeTaken);
                if (currentBest.IsCheckmate) break;
            }
            catch (OperationCanceledException) { break; }
        }
        
        return currentBest;
    }

    private long GetThinkTimeEstimate(long previousThinkTime = 0)
    {
        long estimateThinkTime = previousThinkTime * ComplexityCoefficient;
        return Math.Max(_minimumThinkTime, estimateThinkTime);
    }
    
    private bool EstimatedTimeTooSlow(long thinkTimeEstimate)
    {
        long estimatedEndTime = _timer.MillisecondsElapsedThisTurn + thinkTimeEstimate;
        return estimatedEndTime > _maximumThinkTime;
    }

    private ScoredMove Think(int maxDepth, long thinkTime, out long timeTaken)
    {
        Task<ScoredMove> thinkTask = Task.Run(MoveSearcher);

        Stopwatch s = Stopwatch.StartNew();
        if (!thinkTask.Wait(TimeSpan.FromMilliseconds(thinkTime))) throw new OperationCanceledException($"Ran out of time after thinking for {s.ElapsedMilliseconds}ms.");

        timeTaken = s.ElapsedMilliseconds;
        return thinkTask.Result;

        ScoredMove MoveSearcher() => _searcher.SearchForBestMove(maxDepth);
    }
}