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
            int moveEvaluation = _boardEvaluation.EvaluateMove(move, EvaluationFunction);
            
            ScoredMove scoredMove = new(move, moveEvaluation, maxDepth);
            scoredMoves.Add(scoredMove);
        }
        
        return scoredMoves.Max();
        
        int EvaluationFunction() => -Search(maxDepth);
    }

    private int Search(int depth, int alpha = -999999, int beta = 999999)
    {
        bool depthReached = depth == 0;
        if (!depthReached) return _boardEvaluation.AlphaBetaEvaluateMoves(EvaluationFunction, ref alpha, beta);
        
        bool gameHasEnded = _boardEvaluation.GameHasEnded(out int endEvaluation);
        return gameHasEnded ? endEvaluation : SearchAllCaptures(alpha, beta);

        int EvaluationFunction() => -Search(depth - 1, -beta, -alpha);
    }

    private int SearchAllCaptures(int alpha, int beta)
    {
        int evaluationCurrent = _boardEvaluation.Current;
        if (evaluationCurrent >= beta) return beta;
        if (evaluationCurrent > alpha) alpha = evaluationCurrent;

        return _boardEvaluation.AlphaBetaEvaluateMoves(EvaluationFunction, ref alpha, beta, true);

        int EvaluationFunction() => -SearchAllCaptures(-beta, -alpha);
    }
}