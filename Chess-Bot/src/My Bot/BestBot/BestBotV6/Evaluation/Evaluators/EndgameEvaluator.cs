using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class EndgameEvaluator
{
    internal static float EndgameFactor(this int enemyPieceCount) =>
        (16 - enemyPieceCount) / 15f;

    internal static int CountPieces(this Board board, bool white) =>
        (int)(white ? board.WhitePiecesBitboard : board.BlackPiecesBitboard).HammingWeight();

    private static ulong HammingWeight(this ulong n)
    {
        n -= (n >> 1) & 0x5555555555555555UL;
        n = (n & 0x3333333333333333UL) + ((n >> 2) & 0x3333333333333333UL);
        n = (n + (n >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
        return (n * 0x0101010101010101UL) >> 56;
    }
}