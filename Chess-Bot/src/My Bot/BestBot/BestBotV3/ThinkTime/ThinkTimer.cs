using System;
using System.Collections.Generic;
using System.Threading;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.ThinkTime;

internal class ThinkTimer
{
    internal bool ThinkTimeOver => _cts.IsCancellationRequested;
    
    private readonly int _minimumThinkTime;
    private readonly CancellationTokenSource _cts;
    private readonly List<long> _thinkTimes;
    
    internal ThinkTimer(int minimumThinkTime)
    {
        _minimumThinkTime = minimumThinkTime;
        
        _cts = new();
        _thinkTimes = new();
    }

    internal void AddThinkTimeTaken(int depth, long timeTaken)
    {
        //_thinkTimes
    }

    private int GetNextThinkTime(int previousTimeTaken)
    {
        int nextTimeEstimate = EstimateNextTimeTaken(previousTimeTaken);
        return Math.Max(nextTimeEstimate, _minimumThinkTime);
    }

    private int EstimateNextTimeTaken(int previousTimeTaken)
    {
        return previousTimeTaken * 10;
    }
}