using System.Collections.Generic;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;

internal class BoardEvaluation : BoardWrapper
{
    internal int Current => _currentEvaluation.Perspective(IsWhiteToMove);
    private int _currentEvaluation;
    
    private readonly Stack<int> _moveEvaluationChanges;

    internal BoardEvaluation(Board board) : base(board)
    {
        _moveEvaluationChanges = new();
        _currentEvaluation = EvaluateBoard();
    }

    private protected override void OnMakeMove(Move move)
    {
        base.OnMakeMove(move);
        
        int evalChange = EvaluateMove(move).Perspective(IsWhiteToMove);
        _currentEvaluation += evalChange;
        
        _moveEvaluationChanges.Push(evalChange);
    }

    private protected override void OnUndoMove(Move move)
    {
        base.OnUndoMove(move);
        
        _currentEvaluation -= _moveEvaluationChanges.Pop();
    }
}