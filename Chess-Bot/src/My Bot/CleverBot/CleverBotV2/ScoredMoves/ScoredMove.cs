using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV2.ScoredMoves;

internal struct ScoredMove
{
    internal readonly Move Move;
    internal readonly float Score;

    internal ScoredMove(Move move, float score)
    {
        Move = move;
        Score = score;
    }

    public override string ToString() => $"{Move} | Evaluation: {Score:0.00}";
}