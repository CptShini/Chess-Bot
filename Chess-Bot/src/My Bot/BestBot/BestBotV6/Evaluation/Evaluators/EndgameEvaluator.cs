using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class EndgameEvaluator
{
    private const float magicNumber = 1f / 15f; // I honestly don't even know if this helps (maybe compiler would do this behind the scenes?)
    internal static float EndgameFactor(Board board) => (16 - board.CountEnemyPieces()) * magicNumber;

    private static ulong CountEnemyPieces(this Board board) => (board.IsWhiteToMove ? board.BlackPiecesBitboard : board.WhitePiecesBitboard).HammingWeight();

    private static ulong HammingWeight(this ulong n)
    {
        n -= (n >> 1) & 0x5555555555555555UL;
        n = (n & 0x3333333333333333UL) + ((n >> 2) & 0x3333333333333333UL);
        n = (n + (n >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
        return (n * 0x0101010101010101UL) >> 56;
    }
}