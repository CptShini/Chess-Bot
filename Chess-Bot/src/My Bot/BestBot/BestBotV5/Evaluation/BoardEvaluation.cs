using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation.Evaluators;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation.Evaluators.Evaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal struct BoardEvaluation
{
    private int _currentEvaluation;
    private int _depth;
    
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
        _depth = 0;
    }

    internal Move[] GetMoves(bool capturesOnly = false)
    {
        Move[] moves = _board.GetLegalMoves(capturesOnly);
        moves.GuessOrder();
        return moves;
    }
    
    internal bool GameHasEnded(out int endEvaluation)
    {
        int boardState = _evaluator.EvaluateBoardState(out endEvaluation);
        if (boardState == CheckmateState) endEvaluation += _depth;

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
        _depth++;
        
        int evalChange = _evaluator.EvaluateMove(move) * -Perspective;
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