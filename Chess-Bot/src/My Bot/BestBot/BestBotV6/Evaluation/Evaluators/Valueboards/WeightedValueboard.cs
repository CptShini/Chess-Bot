namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;

internal readonly record struct WeightedValueboard(float[] Values, float Weight, bool Flip = true)
{
    internal float this[int index] => Values[index.FlipIndex(Flip)] * Weight;
}