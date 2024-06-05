using System;
using System.Diagnostics;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Thinking;

internal class Thinker
{
    private const int ExpectedTurnCount = 40;
    private const int SpeedUpTime = 2;
    
    private readonly long _maximumThinkTime;
    private readonly Searcher _searcher;
    private readonly Timer _timer;

    internal Thinker(Board board, Timer timer)
    {
        _searcher = new(board);
        _timer = timer;
        
        int turnCountTime = timer.GameStartTimeMilliseconds / ExpectedTurnCount;
        int emergencyTime = (int)(timer.MillisecondsRemaining / (ExpectedTurnCount / (float)SpeedUpTime));
        _maximumThinkTime = Math.Min(turnCountTime, emergencyTime);
    }

    internal ScoredMove IterativeDeepening()
    {
        ScoredMove currentBest = Think(0, out long timeTaken);
        
        for (int depth = 1; ; depth++)
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