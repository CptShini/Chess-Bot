namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Thinking;

internal class ThinkTimeEstimator
{
    private const int MaxDepth = 32;
    private const int TableLength = 8;
    private const float DefaultBranchFactor = 6f;
    
    private readonly float[,] _branchingTable;
    private readonly int[] _branchingTableIndexers;

    internal ThinkTimeEstimator()
    {
        _branchingTable = new float[MaxDepth, TableLength];
        _branchingTableIndexers = new int[MaxDepth];
    }

    internal void AddBranch(int depth, float branchFactor)
    {
        if (depth >= MaxDepth) return;
        
        int index = _branchingTableIndexers[depth]++;
        if (index == TableLength - 1) _branchingTableIndexers[depth] = 0;
        
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
            if (branchFactor == 0f) continue;
                
            branchFactorSum += branchFactor;
            n++;
        }

        return n == 0 ? DefaultBranchFactor : branchFactorSum / n;
    }

    public override string ToString()
    {
        string result = "";

        for (int i = 0; i < MaxDepth; i++)
        {
            if (GetAverageBranchFactor(i) == 0f) continue;
            result += ToString(i);
        }
        
        return result;
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
}