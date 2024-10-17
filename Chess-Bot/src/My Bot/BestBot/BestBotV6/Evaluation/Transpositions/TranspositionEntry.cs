using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions;

internal readonly record struct TranspositionEntry(ulong Key, int Value, int Depth, int Flag, Move Move)
{
    public override string ToString()
    {
        return $"Key: {Key} | Eval: {Value} | Depth: {Depth} | Flag: {Flag} | Move: {Move}";
    }
}