using System;
using System.Collections.Generic;

namespace CleverBot.CleverBotV3.Evaluations;

internal class Evaluation : IComparable<Evaluation>
{
    internal float Score { get; private set; }
    internal int Depth { get; private set; }

    private readonly bool _white;
    private readonly List<Evaluation> _subEvaluations;

    internal Evaluation(bool white, float score)
    {
        _white = white;

        Score = score;
        Depth = 0;

        _subEvaluations = new();
    }

    internal void AddEvaluation(Evaluation newEvaluation) => _subEvaluations.Add(newEvaluation);

    internal void ReEvaluate()
    {
        if (_subEvaluations.Count == 0) return;

        ResetEvaluation();
        _subEvaluations.ForEach(UpdateEvaluation);
    }

    private void UpdateEvaluation(Evaluation newEvaluation)
    {
        newEvaluation.ReEvaluate();

        if (CompareTo(newEvaluation) == 1)
        {
            Score = newEvaluation.Score;
            Depth = newEvaluation.Depth + 1;
        }
    }

    private void ResetEvaluation() => Score = _white ? float.MinValue : float.MaxValue;

    #region Utility

    public override string ToString() => $"Depth: {Depth} | Evaluation: {Score:0.00}";

    public int CompareTo(Evaluation? other)
    {
        if (other == null) return 1;

        float otherEval = other.Score;

        bool equal = otherEval == Score;
        if (equal) return -Depth.CompareTo(other.Depth);

        bool greater = otherEval > Score;
        bool lower = otherEval < Score;
        return _white && greater || !_white && lower ? 1 : -1;
    }

    #endregion
}