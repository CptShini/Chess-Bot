using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class PieceValueExtensions
{
    internal static int GetPieceValue(this PieceType pieceType) => PieceValues[(int)pieceType];
    private static readonly int[] PieceValues = [
        0, // None
        PawnValue,
        KnightValue,
        BishopValue,
        RookValue,
        QueenValue,
        KingValue
    ];
}