using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal class Searcher
{
    private BoardEvaluation _boardEvaluation;

    internal Searcher(Board board) => _boardEvaluation = new(board);
    
    internal ScoredMove SearchForBestMove(int maxDepth)
    {
        List<ScoredMove> scoredMoves = new();

        Move[] legalMoves = _boardEvaluation.GetMoves();
        foreach (Move move in legalMoves)
        {
            int moveEvaluation = EvaluateMove(move, EvaluationFunction);
            
            ScoredMove scoredMove = new(move, moveEvaluation, maxDepth);
            scoredMoves.Add(scoredMove);
        }
        
        return scoredMoves.Max();
        
        int EvaluationFunction() => -Search(maxDepth);
    }

    private int Search(int depth, int alpha = -999999, int beta = 999999)
    {
        bool gameHasEnded = _boardEvaluation.GameHasEnded(out int endEvaluation);
        if (gameHasEnded) return endEvaluation;
        
        bool depthReached = depth == 0;
        if (depthReached) return QuiescentSearch(alpha, beta);

        Move[] moves = _boardEvaluation.GetMoves();
        foreach (Move move in moves)
        {
            int evaluation = EvaluateMove(move, EvaluationFunction);
            
            if (FailHigh(evaluation, beta)) return beta; // Prune
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
        }

        return alpha;

        int EvaluationFunction() => -Search(depth - 1, -beta, -alpha);
    }

    private int QuiescentSearch(int alpha, int beta)
    {
        int evaluationCurrent = _boardEvaluation.Current;
        if (FailHigh(evaluationCurrent, beta)) return beta; // Prune
        if (!FailLow(evaluationCurrent, alpha)) alpha = evaluationCurrent;

        Move[] moves = _boardEvaluation.GetMoves(true);
        foreach (Move move in moves)
        {
            int evaluation = EvaluateMove(move, EvaluationFunction);
            
            if (FailHigh(evaluation, beta)) return beta; // Prune
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
        }

        return alpha;
        
        int EvaluationFunction() => -QuiescentSearch(-beta, -alpha);
    }

    private int AlphaBetaEvaluateMoves(Func<int> evaluationFunction, ref int alpha, int beta, bool isQuiescent = false)
    {
        Move[] moves = _boardEvaluation.GetMoves(isQuiescent);
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

    private int EvaluateMove(Move move, Func<int> evaluationFunction)
    {
        _boardEvaluation.MakeMove(move);
        int evaluation = evaluationFunction.Invoke();
        _boardEvaluation.UndoMove(move);

        return evaluation;
    }
    
    private static bool FailHigh(int evaluation, int beta) => evaluation >= beta; // Cut-node
    private static bool FailLow(int evaluation, int alpha) => evaluation <= alpha; // All-node
}