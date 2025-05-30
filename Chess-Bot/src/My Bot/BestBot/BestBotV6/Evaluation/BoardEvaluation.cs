using System;
using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Evaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;

internal struct BoardEvaluation
{
    private int _currentEvaluation;
    
    private int _whitePieceCount, _blackPieceCount;
    private float _endgameFactor;
    
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

        _whitePieceCount = _board.CountPieces(true);
        _blackPieceCount = _board.CountPieces(false);
        UpdateEndgameFactor();
        
        _currentEvaluation = _evaluator.EvaluateBoard(_endgameFactor);
    }
    
    internal void PopulateMoves(ref Span<Move> moves, bool capturesOnly) => _board.GetLegalMovesNonAlloc(ref moves, capturesOnly);

    internal void OrderMoves(ref Span<Move> moves, Move pvMove) => _board.OrderMoves(moves, pvMove, _endgameFactor);

    internal bool GameHasEnded(out int endEvaluation)
    {
        GameState gameState = _evaluator.EvaluateBoardState(_endgameFactor, out endEvaluation);
        return gameState switch
        {
            GameState.Checkmate => true,
            GameState.Draw => true,
            GameState.GameNotOver => false,
            _ => throw new ArgumentOutOfRangeException($"This should never happen! {gameState}")
        };
    }
    
    internal void MakeMove(Move move)
    {
        if (move.IsCapture && _board.IsWhiteToMove) _blackPieceCount--;
        if (move.IsCapture && !_board.IsWhiteToMove) _whitePieceCount--;
        UpdateEndgameFactor();
        
        _board.MakeMove(move);
        
        int evalChange = _evaluator.EvaluateMove(move, _endgameFactor) * -Perspective;
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);
        
        if (move.IsCapture && _board.IsWhiteToMove) _blackPieceCount++;
        if (move.IsCapture && !_board.IsWhiteToMove) _whitePieceCount++;
        UpdateEndgameFactor();

        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }

    private void UpdateEndgameFactor()
    {
        int enemyPieceCount = _board.IsWhiteToMove ? _blackPieceCount : _whitePieceCount;
        _endgameFactor = enemyPieceCount.EndgameFactor();
    }
}