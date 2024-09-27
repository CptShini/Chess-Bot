using System;
using System.Text;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal class Line
{
    internal int Depth { get; private set; }       // Number of moves in the line.
    private readonly Move[] Moves = new Move[32];  // The line.

    internal Move this[int index] => Moves[index];

    public override string ToString()
    {
        StringBuilder sb = new(GetMoveName(Moves[0]));

        for (int index = 1; index < Depth; index++)
        {
            if (Moves[index].IsNull) continue;
            
            sb.Append($", {GetMoveName(Moves[index])}");
        }

        return sb.ToString();
    }

    internal void UpdatePVLine(Move pvMove, Line pvLine)
    {
        Moves[0] = pvMove;
        Array.Copy(pvLine.Moves, 0, Moves, 1, pvLine.Depth);
        
        Depth = pvLine.Depth + 1;
    }

    private static string GetMoveName(Move move) => move.ToString().Substring(7, 4);
}