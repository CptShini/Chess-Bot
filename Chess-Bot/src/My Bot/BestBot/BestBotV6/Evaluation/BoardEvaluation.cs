using System;
using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

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

    internal GameState CheckGameState(int plyFromRoot, out int endEvaluation)
    {
        bool isCheckmate = _board.IsInCheckmate();
        if (isCheckmate)
        {
            endEvaluation = CheckmateValue + plyFromRoot;
            return GameState.Checkmate;
        }

        bool isDraw = _board.IsDraw();
        if (isDraw)
        {
            float earlyGameFactor = 1f - _endgameFactor;
            int drawValue = (int)(earlyGameFactor * -ContemptValue);
            endEvaluation = drawValue;
            return GameState.Draw;
        }

        endEvaluation = 0;
        return GameState.GameNotOver;
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

internal enum GameState
{
    GameNotOver = -1,
    Draw = 0,
    Checkmate = 1
}