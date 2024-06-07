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

    internal Move[] GetMoves(bool isQuiescent = false)
    {
        Move[] moves = _board.GetLegalMoves(isQuiescent);
        moves.GuessOrder();
        return moves;
    }
    
    internal bool GameHasEnded(out int endEvaluation)
    {
        endEvaluation = Evaluator.EvaluateBoardState(_board, out bool gameHasEnded);

        bool isCheckmate = endEvaluation == -10000;
        if (isCheckmate) endEvaluation += _depth;
        
        return gameHasEnded;
    }
    
    internal void MakeMove(Move move)
    {
        _board.MakeMove(move);
        _depth++;

        int evalChange = Evaluator.EvaluateMove(move, _board) * -Perspective;
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);
        _depth--;

        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }
    
}