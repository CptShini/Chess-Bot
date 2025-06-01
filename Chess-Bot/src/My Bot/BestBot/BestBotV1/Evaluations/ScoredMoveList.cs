using ChessChallenge.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV1.Evaluations;

internal class ScoredMoveList
{
    private readonly bool _whiteToMove;
    private readonly List<Move> _moves;
    internal float Evaluation { get; private set; }

    internal readonly Dictionary<Move, float> ScoredMoves;

    public ScoredMoveList(bool whiteToMove)
    {
        _whiteToMove = whiteToMove;

        _moves = new();
        Evaluation = whiteToMove ? float.MinValue : float.MaxValue;

        ScoredMoves = new();
    }

    internal void Add(Move move, float moveScore)
    {
        ScoredMoves.Add(move, moveScore);

        if (MoveIsBetter(moveScore))
        {
            Evaluation = moveScore;
            _moves.Clear();
            _moves.Add(move);
        }
        else if (moveScore == Evaluation) _moves.Add(move);
    }

    private bool MoveIsBetter(float moveScore) => _whiteToMove ? moveScore > Evaluation : moveScore < Evaluation;

    internal Move GetMove()
    {
        static T PickRandom<T>(List<T> list) => list[Random.Next(list.Count)];

        return PickRandom(_moves);
    }

    internal Dictionary<Move, float> GetMoves() => ScoredMoves
        .OrderBy(scoredMove => (_whiteToMove ? -1 : 1) * scoredMove.Value).ToDictionary(x => x.Key, y => y.Value);

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"{Evaluation:0.00}");

        foreach (Move move in _moves) sb.AppendLine(move.ToString());

        sb.AppendLine();

        foreach (var scoredMove in GetMoves()) sb.AppendLine($"{scoredMove.Key} | Eval: {scoredMove.Value:0.00}");

        return sb.ToString();
    }
}