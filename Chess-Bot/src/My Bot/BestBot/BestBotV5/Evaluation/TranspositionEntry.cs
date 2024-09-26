using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal readonly record struct TranspositionEntry(ulong Key, int Value, Move Move, int Depth, int Flag)
{
    public override string ToString()
    {
        return $"Key: {Key} | Eval: {Value} | Depth: {Depth} | Flag: {Flag}";
    }
}