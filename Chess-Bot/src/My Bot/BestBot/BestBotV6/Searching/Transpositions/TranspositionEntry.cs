using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Transpositions;

internal readonly record struct TranspositionEntry(ulong Key, int Value, int Depth, TranspositionFlag Flag, Move Move)
{
    public override string ToString() =>
        $"Key: 0x{Key:X16}, Value: {Value,6:+0;-0;0}, Depth: {Depth,2}, Flag: {Flag}, {Move}";
}