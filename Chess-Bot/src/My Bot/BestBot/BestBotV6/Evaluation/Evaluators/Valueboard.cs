namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Valueboard
{
    private readonly int[][] _precomputedValues;

    internal Valueboard(WeightedValueboard valueboard, float lateGameWeight)
        : this(valueboard, valueboard with { Weight = lateGameWeight }) { }
    
    internal Valueboard(WeightedValueboard earlyValueboard, WeightedValueboard lateValueboard)
    {
        _precomputedValues = new int[17][];
        for (int pieceCount = 1; pieceCount <= 16; pieceCount++)
        {
            float endgameFactor = 1.0f - pieceCount / 16f;
            _precomputedValues[pieceCount] = new int[64];
        
            for (int pos = 0; pos < 64; pos++)
            {
                float early = earlyValueboard[pos];
                float late = lateValueboard[pos];
                float value = early + (late - early) * endgameFactor;
                _precomputedValues[pieceCount][pos] = (int)value;
            }
        }
    }

    internal int GetValueAt(int enemyPiecesLeft, int positionIndex) =>
        _precomputedValues[enemyPiecesLeft][positionIndex];
}

internal readonly record struct WeightedValueboard(float[] Values, float Weight, bool Flip = true)
{
    internal float this[int index] => Values[Flip ? 63 - index : index] * Weight;
}