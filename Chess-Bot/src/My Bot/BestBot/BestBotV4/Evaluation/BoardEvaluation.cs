using System;
using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal struct BoardEvaluation
{
    private int _currentEvaluation;
    private int _depth;
    
    internal int Current => _currentEvaluation * Perspective;
    private int Perspective => _board.IsWhiteToMove ? 1 : -1;
    
    private readonly Board _board;
    private readonly Stack<int> _moveEvaluationChanges;
    
    internal BoardEvaluation(Board board)
    {
        _board = board;
        _moveEvaluationChanges = new();

        _currentEvaluation = Evaluator.EvaluateBoard(board);
        _depth = 0;
    }
    
    internal int AlphaBetaEvaluateMoves(Func<int> evaluationFunction, ref int alpha, int beta, bool capturesOnly = false)
    {
        if (!capturesOnly && GameHasEnded(out int endEvaluation)) return endEvaluation;

        Move[] moves = GetMoves(capturesOnly);
        moves.GuessOrder();
        foreach (Move move in moves)
        {
            int evaluation = EvaluateMove(move, evaluationFunction);
            
            if (FailHigh(evaluation, beta)) return beta; // Prune
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
        }

        return alpha;
    }

    internal Move[] GetMoves(bool capturesOnly = false) => _board.GetLegalMoves(capturesOnly);
    
    internal bool GameHasEnded(out int endEvaluation)
    {
        endEvaluation = Evaluator.EvaluateBoardState(_board, out bool gameHasEnded);

        bool isCheckmate = endEvaluation == -10000;
        if (isCheckmate) endEvaluation += _depth;
        
        return gameHasEnded;
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
        _depth++;

        int evalChange = Evaluator.EvaluateMove(move, _board) * -Perspective;
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    private void UndoMove(Move move)
    {
        _board.UndoMove(move);
        _depth--;

        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }

    private static bool FailHigh(int evaluation, int beta) => evaluation >= beta; // Cut-node
    private static bool FailLow(int evaluation, int alpha) => evaluation <= alpha; // All-node
}