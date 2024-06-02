using System.Text;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Valueboards;

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
            valueboard[i] = weightedValueboard[i] / 100f;
        }

        return valueboard;
    }

    internal float GetValueAt(float endgameFactor, int positionIndex)
    {
        float earlyValue = _earlyGameValueboard[positionIndex];
        float lateValue = _lateGameValueboard[positionIndex];
        float valueAt = Lerp(earlyValue, lateValue, endgameFactor);

        return valueAt;
    }

    private static float Lerp(float a, float b, float f) => a * (1f - f) + b * f;

    public string ToString(float endgameFactor)
    {
        StringBuilder sb = new();

        for (int i = 0; i < 64; i++)
        {
            float val = GetValueAt(endgameFactor, i) * 100f;
            sb.Append($"{val:0.0}\t");
            if ((i + 1) % 8 == 0) sb.AppendLine();
        }

        return sb.ToString();
    }
}