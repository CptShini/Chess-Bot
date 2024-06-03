using System;
using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;

internal struct BoardEvaluation
{
    private int _currentEvaluation;
    
    internal int Current => _currentEvaluation * Perspective;
    private int Perspective => _board.IsWhiteToMove ? 1 : -1;
    
    private readonly Board _board;
    private readonly Stack<int> _moveEvaluationChanges;
    
    internal BoardEvaluation(Board board)
    {
        _board = board;
        _moveEvaluationChanges = new();

        _currentEvaluation = Evaluator.EvaluateBoard(board);
    }

    internal bool GameHasEnded(out int endEvaluation) => GameHasEnded(_board.GetLegalMoves(), out endEvaluation);
    private bool GameHasEnded(Move[] legalMoves, out int endEvaluation)
    {
        endEvaluation = 0;
        
        bool noLegalMoves = legalMoves.Length == 0;
        if (!noLegalMoves) return false;
        
        endEvaluation = Evaluator.EvaluateBoardState(_board, out bool gameHasEnded);
        return gameHasEnded;
    }
    
    internal int AlphaBetaEvaluateMoves(Func<int> evaluationFunction, ref int alpha, int beta, bool capturesOnly = false)
    {
        Move[] moves = _board.GetLegalMoves(capturesOnly);
        
        if (!capturesOnly && GameHasEnded(moves, out int endEvaluation)) return endEvaluation;
        
        foreach (Move move in moves.GuessOrder())
        {
            int evaluation = EvaluateMove(move, evaluationFunction);
            if (evaluation >= beta) return beta;
            if (evaluation > alpha) alpha = evaluation;
        }

        return alpha;
    }

    internal int EvaluateMove(Move move, Func<int> evaluationFunction)
    {
        MakeMove(move);
        int evaluation = evaluationFunction.Invoke();
        UndoMove(move);

        return evaluation;
    }
    
    private void MakeMove(Move move)
    {
        _board.MakeMove(move);

        int evalChange = Evaluator.EvaluateMove(move, _board) * -Perspective;
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    private void UndoMove(Move move)
    {
        _board.UndoMove(move);

        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }
}