namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;

internal class Valueboard
{
    private readonly float[] _earlyGameValueboard, _lateGameValueboard;

    internal Valueboard(WeightedValueboard earlyValueboard, WeightedValueboard lateValueboard)
    {
        _earlyGameValueboard = ConvertWeightedValueboard(earlyValueboard);
        _lateGameValueboard = ConvertWeightedValueboard(lateValueboard);
    }

    internal Valueboard(WeightedValueboard valueboard, float lateGameWeight)
    {
        _earlyGameValueboard = ConvertWeightedValueboard(valueboard);
        _lateGameValueboard = ConvertWeightedValueboard(valueboard with { Weight = lateGameWeight });
    }

    private static float[] ConvertWeightedValueboard(WeightedValueboard weightedValueboard)
    {
        float[] valueboard = new float[64];
        for (int i = 0; i < 64; i++)
        {
            valueboard[i] = weightedValueboard[i];
        }

        return valueboard;
    }

    internal int GetValueAt(float endgameFactor, int positionIndex)
    {
        float early = _earlyGameValueboard[positionIndex];
        float late = _lateGameValueboard[positionIndex];
        return (int)(early + (late - early) * endgameFactor);
    }
}