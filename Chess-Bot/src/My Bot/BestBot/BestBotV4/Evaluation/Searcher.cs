using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal class Searcher
{
    private BoardEvaluation _boardEvaluation;

    internal Searcher(Board board) => _boardEvaluation = new(board);

    internal int Search(Line line, int depth, int alpha = -999999, int beta = 999999)
    {
        bool gameHasEnded = _boardEvaluation.GameHasEnded(out int endEvaluation);
        if (gameHasEnded) return endEvaluation;
        
        bool depthReached = depth == 0;
        if (depthReached) return QuiescentSearch(alpha, beta);

        Line newLine = new();
        
        Move[] moves = _boardEvaluation.GetMoves();
        foreach (Move move in moves)
        {
            _boardEvaluation.MakeMove(move);
            int evaluation = -Search(newLine, depth - 1, -beta, -alpha);
            _boardEvaluation.UndoMove(move);
            
            if (FailHigh(evaluation, beta)) return beta; // Prune
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
            UpdateLine(move);
        }

        return alpha;

        void UpdateLine(Move move)
        {
            line.Moves[0] = move;
            Array.Copy(newLine.Moves, 0, line.Moves, 1, newLine.Depth);
            line.Depth = newLine.Depth + 1;
        }
    }

    private int QuiescentSearch(int alpha, int beta)
    {
        int evaluationCurrent = _boardEvaluation.Current;
        if (FailHigh(evaluationCurrent, beta)) return beta; // Prune
        if (!FailLow(evaluationCurrent, alpha)) alpha = evaluationCurrent;

        Move[] moves = _boardEvaluation.GetMoves(true);
        foreach (Move move in moves)
        {
            _boardEvaluation.MakeMove(move);
            int evaluation = -QuiescentSearch(-beta, -alpha);
            _boardEvaluation.UndoMove(move);
            
            if (FailHigh(evaluation, beta)) return beta; // Prune
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
        }

        return alpha;
    }

    private int AlphaBetaEvaluateMoves(Func<int> evaluationFunction, Action<Move>? actionOnPvNodeFound, ref int alpha, int beta, bool isQuiescent = false)
    {
        Move[] moves = _boardEvaluation.GetMoves(isQuiescent);
        foreach (Move move in moves)
        {
            _boardEvaluation.MakeMove(move);
            int evaluation = evaluationFunction();
            _boardEvaluation.UndoMove(move);
            
            if (FailHigh(evaluation, beta)) return beta; // Prune
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
            actionOnPvNodeFound?.Invoke(move);
        }

        return alpha;
    }
    
    private static bool FailHigh(int evaluation, int beta) => evaluation >= beta; // Cut-node
    private static bool FailLow(int evaluation, int alpha) => evaluation <= alpha; // All-node
}