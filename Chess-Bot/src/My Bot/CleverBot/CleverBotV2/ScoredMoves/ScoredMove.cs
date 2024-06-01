using ChessChallenge.API;

namespace CleverBot.CleverBotV2.ScoredMoves;

internal struct ScoredMove
{
    internal readonly Move Move;
    internal readonly float Score;

    internal ScoredMove(Move move, float score)
    {
        Move = move;
        Score = score;
    }

    public override string ToString() => $"{Move} | Eval: {Score:0.00}";
}