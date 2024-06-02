using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal static class EndgameEvaluator
{
    internal static float EndgameFactor(Board board) => (16 - board.CountEnemyPieces()) / 15f;

    private static int CountEnemyPieces(this Board board) => (board.IsWhiteToMove ? board.BlackPiecesBitboard : board.WhitePiecesBitboard).HammingWeight();

    private static int HammingWeight(this ulong n)
    {
        n -= (n >> 1) & 0x5555555555555555UL;
        n = (n & 0x3333333333333333UL) + ((n >> 2) & 0x3333333333333333UL);
        n = (n + (n >> 4)) & 0x0F0F0F0F0F0F0F0FUL;
        return (int)((n * 0x0101010101010101UL) >> 56);
    }
}