﻿namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators.Valueboards;

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

    internal int GetValueAt(float endgameFactor, int positionIndex)
    {
        float earlyValue = _earlyGameValueboard[positionIndex];
        float lateValue = _lateGameValueboard[positionIndex];
        int valueAt = (int)Lerp(earlyValue, lateValue, endgameFactor);

        return valueAt;
    }

    private static float Lerp(float a, float b, float f) => a * (1f - f) + b * f;
}