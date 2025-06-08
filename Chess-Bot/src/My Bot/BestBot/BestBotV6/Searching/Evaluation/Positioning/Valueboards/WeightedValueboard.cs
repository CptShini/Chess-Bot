namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning.Valueboards;

internal readonly record struct WeightedValueboard(float[] Values, float Weight, bool Flip = true)
{
    internal float this[int index] => Values[index.FlipIndex(Flip)] * Weight;
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine($"WeightedValueboard (Weight = {Weight}, Flip = {Flip}):");
        for (int rank = 7; rank >= 0; rank--)
        {
            sb.Append("  ");
            for (int file = 0; file < 8; file++)
            {
                int index = rank * 8 + file;
                float weightedValue = this[index];
                sb.Append($"{weightedValue,4:0} ");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}