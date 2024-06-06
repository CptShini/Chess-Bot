namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators.Valueboards;

internal readonly record struct WeightedValueboard(float[] Values, float Weight, bool Flip = true)
{
    internal float this[int index] => Values[Flip ? 63 - index : index] * Weight;
}