namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Valueboards;

internal record struct WeightedValueboard(float[] Values, float Weight, bool Flip = true)
{
    internal float this[int index] => Values[Flip ? 63 - index : index] * Weight;
}