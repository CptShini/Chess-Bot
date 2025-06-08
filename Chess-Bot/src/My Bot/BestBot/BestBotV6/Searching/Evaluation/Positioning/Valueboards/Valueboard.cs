namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning.Valueboards;

internal class Valueboard
{
    private const int
        MaxPieceCount = 16,
        SquareCount = 64;
    
    private readonly int[] _precomputedValueboards;

    internal Valueboard(WeightedValueboard valueboard, float lateGameWeight)
        : this(valueboard, valueboard with { Weight = lateGameWeight }) { }
    
    internal Valueboard(WeightedValueboard earlyValueboard, WeightedValueboard lateValueboard)
    {
        _precomputedValueboards = new int[(MaxPieceCount + 1) * SquareCount];
        for (int pieceCount = MaxPieceCount; pieceCount > 0; pieceCount--)
        {
            PrecomputeForPieceCount(pieceCount);
        }

        return;

        void PrecomputeForPieceCount(int pieceCount)
        {
            float endgameFactor = 1.0f - (pieceCount - 1f) / (MaxPieceCount - 1);

            int offset = pieceCount * SquareCount;
            for (int square = 0; square < SquareCount; square++)
            {
                int index = offset + square;
                int value = ComputeInterpolatedValue(square, endgameFactor);
                _precomputedValueboards[index] = value;
            }
        }

        int ComputeInterpolatedValue(int square, float endgameFactor)
        {
            float earlyValue = earlyValueboard[square];
            float lateValue = lateValueboard[square];
            float interpolated = Lerp(earlyValue, lateValue, endgameFactor);
            return (int)interpolated;
        }
        
        static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
    
    internal int this[int enemyPieceCount, int positionIndex]
    {
        get
        {
            int offset = enemyPieceCount << 6; // Same as multiplying by 64
            int index = offset + positionIndex;
            return _precomputedValueboards[index];
        }
    }
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (int pieceCount = MaxPieceCount; pieceCount > 0; pieceCount--)
        {
            sb.AppendLine(ToString(pieceCount));
        }
        return sb.ToString();
    }
    
    public string ToString(int enemyPieceCount)
    {
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine($"Valueboard for enemyPieceCount = {enemyPieceCount}:");
        for (int rank = 7; rank >= 0; rank--)
        {
            sb.Append("  ");
            for (int file = 0; file < 8; file++)
            {
                int index = rank * 8 + file;
                int value = this[enemyPieceCount, index];
                sb.Append($"{value,4} ");
            }

            sb.AppendLine();
        }
        
        return sb.ToString();
    }
}