using System.Collections.Generic;
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
                const float DefaultBranchFactor = 6f;
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
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("=== Think Time Estimator ===");
        sb.AppendLine("Depth | Avg BF | Recent Branch Factors");
        sb.AppendLine("------+--------+------------------------");

        for (int depth = 1; depth < DepthLimit; depth++)
        {
            LinkedList<float> entries = _branchingTable[depth];
            if (entries.Count == 0) break;
            
            float avg = GetAverageBranchFactor(depth);
            sb.Append($"  {depth,2}  | {avg,6:0.00} |");

            foreach (float bf in entries)
            {
                sb.Append($" {bf,6:0.00}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}