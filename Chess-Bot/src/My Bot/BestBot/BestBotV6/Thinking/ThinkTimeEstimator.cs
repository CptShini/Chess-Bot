using System;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;

internal class ThinkTimeEstimator
{
    private const int MaxDepth = 32;
    private const int TableLength = 5;
    private const float DefaultBranchFactor = 6f;
    
    private readonly float[,] _branchingTable;
    private readonly int[] _branchingTableIndexers;

    internal ThinkTimeEstimator()
    {
        _branchingTable = new float[MaxDepth, TableLength];
        _branchingTableIndexers = new int[MaxDepth];
    }
    
    internal void AddBranch(int depth, long current, long previous)
    {
        if (depth is >= MaxDepth or <= 0) return;
        
        int index = _branchingTableIndexers[depth]++;
        if (index == TableLength - 1) _branchingTableIndexers[depth] = 0;

        float branchFactor = (float)current / previous;
        _branchingTable[depth, index] = branchFactor;
    }

    internal float GetAverageBranchFactor(int depth)
    {
        if (depth >= MaxDepth) return DefaultBranchFactor;

        float branchFactorSum = 0f;

        int n = 0;
        for (int i = 0; i < TableLength; i++)
        {
            float branchFactor = _branchingTable[depth, i];
            if (branchFactor == 0f) break;
                
            branchFactorSum += branchFactor;
            n++;
        }

        return n == 0 ? DefaultBranchFactor : branchFactorSum / n;
    }

    private string ToString(int depth)
    {
        string result = $"{depth} | {GetAverageBranchFactor(depth):0.00} | ";

        for (int i = 0; i < TableLength; i++)
        {
            result += $"{_branchingTable[depth, i]:0.00} ";
        }
        
        result += "\n";

        return result;
    }
    
    public override string ToString()
    {
        string result = "";

        float prevBranchFactor = -1f;
        for (int i = 1; i < MaxDepth; i++)
        {
            float branchFactor = GetAverageBranchFactor(i);
            
            if (Math.Abs(branchFactor - prevBranchFactor) < 0.001f) break;
            result += ToString(i);

            prevBranchFactor = branchFactor;
        }
        
        return result;
    }
}