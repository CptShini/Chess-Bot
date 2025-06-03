namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;

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
            float endgameFactor = 1.0f - (float)pieceCount / MaxPieceCount;

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
}