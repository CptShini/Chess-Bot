namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;

internal class Valueboard
{
    private readonly float[] _earlyGameValueboard, _lateGameValueboard;

    internal Valueboard(WeightedValueboard earlyValueboard, WeightedValueboard lateValueboard)
    {
        _earlyGameValueboard = ConvertWeightedValueboard(earlyValueboard);
        _lateGameValueboard = ConvertWeightedValueboard(lateValueboard);
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

    internal int GetValueAt(float endgameFactor, int positionIndex) => (int)Lerp(_earlyGameValueboard[positionIndex], _lateGameValueboard[positionIndex], endgameFactor);

    private static float Lerp(float a, float b, float f) => a * (1f - f) + b * f;
}