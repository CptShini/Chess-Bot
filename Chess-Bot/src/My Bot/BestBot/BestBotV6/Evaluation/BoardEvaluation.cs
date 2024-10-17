using System;
using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Evaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;

internal struct BoardEvaluation
{
    private int _currentEvaluation;
    
    internal int Current => _currentEvaluation * Perspective;
    private int Perspective => _board.IsWhiteToMove ? 1 : -1;
    
    private readonly Board _board;
    private readonly Evaluator _evaluator;
    private readonly Stack<int> _moveEvaluationChanges;
    
    internal BoardEvaluation(Board board)
    {
        _board = board;
        _evaluator = new(board);
        _moveEvaluationChanges = new();

        _currentEvaluation = _evaluator.EvaluateBoard();
    }
    
    internal void PopulateMoves(ref Span<Move> moves, bool capturesOnly) => _board.GetLegalMovesNonAlloc(ref moves, capturesOnly);

    internal void OrderMoves(ref Span<Move> moves, Move pvMove) => MoveOrderer.OrderMoves(_board, moves, pvMove);

    internal bool GameHasEnded(out int endEvaluation)
    {
        int boardState = _evaluator.EvaluateBoardState(out endEvaluation);
        return boardState switch
        {
            CheckmateState => true,
            DrawState => true,
            _ => false
        };
    }
    
    internal void MakeMove(Move move)
    {
        _board.MakeMove(move);
        
        int evalChange = _evaluator.EvaluateMove(move) * -Perspective;
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);

        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }
}