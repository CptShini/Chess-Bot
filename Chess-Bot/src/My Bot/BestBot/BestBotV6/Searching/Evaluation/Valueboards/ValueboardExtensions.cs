using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Valueboards;

internal static class ValueboardExtensions
{
    internal static Valueboard GetValueboard(this PieceType pieceType) => Valueboards[(int)pieceType];
    private static readonly Valueboard[] Valueboards = [
        null!, // None
        PawnValueboard,
        KnightValueboard,
        BishopValueboard,
        RookValueboard,
        QueenValueboard,
        KingValueboard
    ];
}