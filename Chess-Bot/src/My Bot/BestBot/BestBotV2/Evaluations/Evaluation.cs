using Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;
using ChessChallenge.API;
using System;
using System.Collections.Generic;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations;

internal struct Evaluation
{
    internal float Current { get; private set; }
    internal bool GameHasEnded { get; private set; }
    internal bool IsMax => _board.IsWhiteToMove;

    private readonly Board _board;
    private readonly Stack<float> _moveEvaluationsChanges;

    internal Evaluation(Board board)
    {
        _board = board;
        _moveEvaluationsChanges = new Stack<float>();

        (Current, GameHasEnded) = Evaluator.EvaluateBoard(board);
    }

    internal float EvaluateMove(Move move, Func<float> evaluationFunction)
    {
        MakeMove(move);
        float eval = evaluationFunction();
        UndoMove(move);

        return eval;
    }

    internal void MakeMove(Move move)
    {
        _board.MakeMove(move);

        (float evalChange, GameHasEnded) = Evaluator.EvaluateMove(_board, move);
        if (GameHasEnded) evalChange -= Current;

        Current += evalChange;
        _moveEvaluationsChanges.Push(evalChange);
    }

    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);

        Current -= _moveEvaluationsChanges.Pop();
        GameHasEnded = false;
    }
}