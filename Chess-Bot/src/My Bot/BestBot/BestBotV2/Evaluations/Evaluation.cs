using System.Collections.Generic;
using ChessChallenge.API;
using Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations;

internal struct Evaluation
{
    internal float Current { get; private set; }
    internal bool GameHasEnded { get; private set; }

    private readonly Board _board;
    private readonly Evaluator _evaluator;
    private readonly Stack<float> _moveEvaluationChanges;
    private readonly Dictionary<ulong, (float, bool)> _transpositionTable;

    internal Evaluation(Board board)
    {
        _board = board;
        _evaluator = new(board);
        _moveEvaluationChanges = new();
        _transpositionTable = new();

        (Current, GameHasEnded) = _evaluator.EvaluateBoard();
    }

    internal void MakeMove(Move move, bool tradeMove = false)
    {
        _board.MakeMove(move);

        (float evalChange, GameHasEnded) = EvaluateMove(move, tradeMove);
        if (GameHasEnded) evalChange -= Current;

        Current += evalChange;
        _moveEvaluationChanges.Push(evalChange);
    }

    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);

        Current -= _moveEvaluationChanges.Pop();
        GameHasEnded = false;
    }

    private (float, bool) EvaluateMove(Move move, bool tradeMove)
    {
        ulong zobristKey = _board.ZobristKey;
        bool isTransposition = _transpositionTable.TryGetValue(zobristKey, out var transposition);
        if (isTransposition) return transposition;

        transposition = !tradeMove ? _evaluator.EvaluateMove(move) : (_evaluator.EvaluateOnlyMove(move), false);
        _transpositionTable.Add(zobristKey, transposition);

        return transposition;
    }
}