using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal struct Line
{
    internal int Depth;              // Number of moves in the line.
    internal readonly Move[] Moves;  // The line.

    public Line()
    {
        Depth = 0;
        Moves = new Move[64];
    }

    public override string ToString() => $"{Moves[0]} | Depth: {Depth}";
}