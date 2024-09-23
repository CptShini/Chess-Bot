using System.Text;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal class Line
{
    internal int Depth;              // Number of moves in the line.
    internal readonly Move[] Moves;  // The line.

    public Line()
    {
        Depth = 0;
        Moves = new Move[32];
    }
    
    public override string ToString()
    {
        StringBuilder sb = new(GetMoveName(Moves[0]));

        for (int index = 1; index < Moves.Length; index++)
        {
            if (Moves[index].IsNull) continue;
            
            sb.Append($", {GetMoveName(Moves[index])}");
        }

        return sb.ToString();
    }

    private static string GetMoveName(Move move) => move.ToString().Substring(7, 4);
}