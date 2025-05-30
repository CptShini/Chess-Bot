using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class EndgameEvaluator
{
    internal static float EndgameFactor(this int enemyPieceCount) =>
        (16 - enemyPieceCount) / 15f;

    internal static int CountPieces(this Board board, bool white) =>
        (white ? board.WhitePiecesBitboard : board.BlackPiecesBitboard).HammingWeight();

    private static int HammingWeight(this ulong n) =>
        System.Numerics.BitOperations.PopCount(n);
}