using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal struct BoardEvaluation
{
    private const int CheckmateValue = -10000;
    private const int ContemptValue = 50;
    
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
        int boardState = Evaluator.EvaluateBoardState(_board);
        switch (boardState)
        {
            case 1:
                endEvaluation = (int)((1f - EndgameEvaluator.EndgameFactor(_board)) * ContemptValue);
                return true;
            case 2:
                endEvaluation = CheckmateValue + _depth;
                return true;
            default:
                endEvaluation = 0;
                return false;
        }
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