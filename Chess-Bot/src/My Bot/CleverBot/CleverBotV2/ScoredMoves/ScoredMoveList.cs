﻿using System.Collections.Generic;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV2.ScoredMoves;

internal struct ScoredMoveList
{
    private readonly System.Random _random = new();

    private readonly bool _white;
    private readonly List<Move> _moves;

    private float _eval;

    internal ScoredMove Move => new(_moves[_random.Next(_moves.Count)], _eval);

    internal ScoredMoveList(bool white)
    {
        _white = white;
        _moves = new();

        _eval = white ? float.MinValue : float.MaxValue;
    }

    internal void Add(Move move, float moveScore)
    {
        if (EvalIsBetter(moveScore))
        {
            _eval = moveScore;
            _moves.Clear();
            _moves.Add(move);
        }
        else if (moveScore == _eval) _moves.Add(move);
    }

    private bool EvalIsBetter(float newEval)
    {
        bool greater = newEval > _eval;
        bool lower = newEval < _eval;

        return _white && greater || !_white && lower;
    }
}