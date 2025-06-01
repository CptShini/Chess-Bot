using ChessChallenge.API;
using System.Collections.Generic;
using System.Text;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV1;

internal struct ScoredMoveList
{
    private static readonly System.Random _random = new();

    private readonly List<Move> _maxMoves;
    private readonly List<Move> _minMoves;
    private float _minEval;
    private float _maxEval;

    public ScoredMoveList()
    {
        _minEval = float.MaxValue;
        _maxEval = float.MinValue;
        _maxMoves = new();
        _minMoves = new();
    }

    internal void Add(Move move, float moveScore)
    {
        if (moveScore > _maxEval)
        {
            _maxEval = moveScore;
            _maxMoves.Clear();
            _maxMoves.Add(move);
        }
        else if (moveScore == _maxEval) _maxMoves.Add(move);

        if (moveScore < _minEval)
        {
            _minEval = moveScore;
            _minMoves.Clear();
            _minMoves.Add(move);
        }
        else if (moveScore == _minEval) _minMoves.Add(move);
    }

    internal Move GetMove(bool whiteToMove)
    {
        static T PickRandom<T>(List<T> list) => list[_random.Next(list.Count)];

        return whiteToMove ? PickRandom(_maxMoves) : PickRandom(_minMoves);
    }

    internal float GetEval(bool whiteToMove) => whiteToMove ? _maxEval : _minEval;

    internal string ToString(bool whiteToMove)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{(whiteToMove ? _maxEval : _minEval):0.0}");

        List<Move> bestMoves = whiteToMove ? _maxMoves : _minMoves;
        foreach (Move move in bestMoves) sb.AppendLine(move.ToString());

        return sb.ToString();
    }
}