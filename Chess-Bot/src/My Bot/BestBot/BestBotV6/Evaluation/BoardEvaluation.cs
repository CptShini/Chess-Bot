using System;
using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;

internal struct BoardEvaluation
{
    internal int Current => _currentEvaluation.Perspective(_board.IsWhiteToMove);
    private int _currentEvaluation;
    
    private int _enemyPiecesLeft => _board.IsWhiteToMove ? _blackPieceCount : _whitePieceCount;
    private int _whitePieceCount, _blackPieceCount;
    
    private readonly Board _board;
    private readonly Evaluator _evaluator;
    private readonly Stack<int> _moveEvaluationChanges;
    
    internal BoardEvaluation(Board board)
    {
        _board = board;
        _evaluator = new(board);
        _moveEvaluationChanges = new();

        _whitePieceCount = CountPieces(true);
        _blackPieceCount = CountPieces(false);
        
        _currentEvaluation = _evaluator.EvaluateBoard(_enemyPiecesLeft);

        return;
        
        int CountPieces(bool white)
        {
            ulong bitBoard = white ? board.WhitePiecesBitboard : board.BlackPiecesBitboard;
            return System.Numerics.BitOperations.PopCount(bitBoard);
        }
    }
    
    internal void PopulateMoves(ref Span<Move> moves, bool capturesOnly) => _board.GetLegalMovesNonAlloc(ref moves, capturesOnly);

    internal void OrderMoves(ref Span<Move> moves, Move pvMove) => _board.OrderMoves(moves, pvMove, _enemyPiecesLeft);

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
            float earlyGameFactor = _enemyPiecesLeft / 16f;
            int drawValue = (int)(earlyGameFactor * -ContemptValue);
            endEvaluation = drawValue;
            return GameState.Draw;
        }

        endEvaluation = 0;
        return GameState.GameNotOver;
    }
    
    internal void MakeMove(Move move)
    {
        bool isWhiteToMove = _board.IsWhiteToMove;
        if (move.IsCapture)
        {
            if (isWhiteToMove) _blackPieceCount--;
            else _whitePieceCount--;
        }
        
        _board.MakeMove(move);

        int evalChange = _evaluator.EvaluateMove(move, _enemyPiecesLeft).Perspective(isWhiteToMove);
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);
        
        if (move.IsCapture)
        {
            if (_board.IsWhiteToMove) _blackPieceCount++;
            else _whitePieceCount++;
        }

        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }
}

internal enum GameState
{
    GameNotOver = -1,
    Draw = 0,
    Checkmate = 1
}