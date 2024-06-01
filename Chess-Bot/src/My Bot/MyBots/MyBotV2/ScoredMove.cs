using ChessChallenge.API;

namespace MyBots.MyBotV2;

public struct ScoredMove
{
    public Move Move;
    public float Score;

    public ScoredMove(Move move, float score)
    {
        Move = move;
        Score = score;
    }
}