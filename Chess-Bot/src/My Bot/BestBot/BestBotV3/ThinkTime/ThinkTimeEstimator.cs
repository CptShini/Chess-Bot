using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.ThinkTime;

internal class ThinkTimeEstimator
{
    private readonly Dictionary<int, Queue<long>?> _thinkTimeModel;
    private readonly int _entryCount;

    internal ThinkTimeEstimator(int entryCount)
    {
        _entryCount = entryCount;
        _thinkTimeModel = new();
    }

    internal void AddTimeTakenEntry(int depth, long timeTaken)
    {
        if (!HasModelEntry(depth, out Queue<long> depthModel))
        {
            depthModel = new();
            _thinkTimeModel.Add(depth, depthModel);
        }
        
        depthModel.Enqueue(timeTaken);
        if (depthModel.Count > _entryCount) depthModel.Dequeue();
    }

    internal long GetThinkTimeEstimate(int depth)
    {
        if (!HasModelEntry(depth, out Queue<long> depthModel)) return -1;

        return (long)depthModel.Average();
    }

    private bool HasModelEntry(int depth, out Queue<long> depthModel) => _thinkTimeModel.TryGetValue(depth, out depthModel) && depthModel != null;
}