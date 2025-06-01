namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Valueboard
{
    private const int
        maxPieceCount = 16,
        squareCount = 64;
    
    private readonly int[] _precomputedValues;

    internal Valueboard(WeightedValueboard valueboard, float lateGameWeight)
        : this(valueboard, valueboard with { Weight = lateGameWeight }) { }
    
    internal Valueboard(WeightedValueboard earlyValueboard, WeightedValueboard lateValueboard)
    {
        _precomputedValues = new int[(maxPieceCount + 1) * squareCount];
        for (int pieceCount = 1; pieceCount <= maxPieceCount; pieceCount++)
        {
            PrecomputeForPieceCount(pieceCount);
        }

        return;

        void PrecomputeForPieceCount(int pieceCount)
        {
            float endgameFactor = 1.0f - (float)pieceCount / maxPieceCount;

            int offset = pieceCount * squareCount;
            for (int square = 0; square < squareCount; square++)
            {
                int index = offset + square;
                int value = ComputeInterpolatedValue(square, endgameFactor);
                _precomputedValues[index] = value;
            }
        }

        int ComputeInterpolatedValue(int square, float endgameFactor)
        {
            float earlyValue = earlyValueboard[square];
            float lateValue = lateValueboard[square];
            float interpolated = Lerp(earlyValue, lateValue, endgameFactor);
            return (int)interpolated;
        }
        
        float Lerp(float a, float b, float t) =>
            a + (b - a) * t;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    internal int GetValueAt(int enemyPiecesLeft, int positionIndex) =>
        _precomputedValues[(enemyPiecesLeft << 6) + positionIndex];
}

internal readonly record struct WeightedValueboard(float[] Values, float Weight, bool Flip = true)
{
    internal float this[int index] => Values[Flip ? 63 - index : index] * Weight;
}