using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;

internal class ThinkTimeEstimator
{
    private readonly LinkedList<float>[] _branchingTable;

    public ThinkTimeEstimator()
    {
        _branchingTable = new LinkedList<float>[DepthLimit];
        for (int i = 0; i < DepthLimit; i++) _branchingTable[i] = [];
    }

    internal void AddBranch(int depth, long current, long previous)
    {
        if (depth is >= DepthLimit or <= 0) return;

        float branchFactor = (float)current / previous;
        
        LinkedList<float> branchingList = _branchingTable[depth];
        branchingList.AddFirst(branchFactor);

        if (branchingList.Count <= TableLength) return;
        branchingList.RemoveLast();
    }

    internal float GetAverageBranchFactor(int depth)
    {
        switch (depth)
        {
            case <= 0:
                return DefaultBranchFactor;
            case >= DepthLimit:
                return GetAverageBranchFactor(DepthLimit - 1);
        }

        float branchFactorSum = 0f, weightSum = 0f;
        
        int n = 0;
        foreach (float branchFactor in _branchingTable[depth])
        {
            float weight = BranchFactorRecencyWeight(n);
            weightSum += weight;
            
            branchFactorSum += branchFactor * weight;
            n++;
        }

        if (n > 0) return branchFactorSum / weightSum;

        float previousBranchFactor1 = GetAverageBranchFactor(depth - 1);
        float previousBranchFactor2 = GetAverageBranchFactor(depth - 2);
        float averageNextBranchFactor = (previousBranchFactor1 + previousBranchFactor2) / 2f;
        return averageNextBranchFactor;
    }

    private string ToString(int depth)
    {
        StringBuilder sb = new();
        
        sb.Append($"{depth.ToString().PadLeft(2)} | {GetAverageBranchFactor(depth).ToString("F", CultureInfo.InvariantCulture).PadLeft(5)} | ");

        foreach (float branchFactor in _branchingTable[depth])
        {
            sb.Append($"{branchFactor.ToString("F", CultureInfo.InvariantCulture).PadLeft(5)} ");
        }
        
        sb.AppendLine();

        return sb.ToString();
    }
    
    public override string ToString()
    {
        StringBuilder sb = new();

        for (int i = 1; i < DepthLimit; i++)
        {
            sb.Append(ToString(i));
            
            if (_branchingTable[i].Count == 0) break;
        }
        
        return sb.ToString();
    }
}