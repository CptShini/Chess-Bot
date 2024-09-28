using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal class Searcher
{
    private BoardEvaluation _boardEvaluation;
    private readonly TranspositionTable _transpositionTable;
    
    internal Move BestMove { get; private set; }

    internal Searcher(Board board)
    {
        _boardEvaluation = new(board);
        _transpositionTable = new(board, 16);
        BestMove = Move.NullMove;
    }

    internal int Search(int plyRemaining, int plyFromRoot = 0, int alpha = -999999, int beta = 999999)
    {
        bool gameHasEnded = _boardEvaluation.GameHasEnded(out int endEvaluation);
        if (gameHasEnded) return endEvaluation;

        int ttVal = _transpositionTable.LookupEvaluation(plyRemaining, alpha, beta);
        if (ttVal != TranspositionTable.LookupFailed) return ttVal;
        
        bool depthReached = plyRemaining == 0;
        if (depthReached) return QuiescentSearch(alpha, beta);

        int evaluationFlag = TranspositionTable.FlagAlpha;
        
        Move[] moves = _boardEvaluation.GetMoves();
        foreach (Move move in moves)
        {
            _boardEvaluation.MakeMove(move);
            int evaluation = -Search(plyRemaining - 1, plyFromRoot + 1, -beta, -alpha);
            _boardEvaluation.UndoMove(move);

            if (FailHigh(evaluation, beta)) // Prune
            {
                _transpositionTable.StoreEvaluation(plyRemaining, beta, TranspositionTable.FlagBeta);
                return beta;
            }
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
            evaluationFlag = TranspositionTable.FlagExact;
            if (plyFromRoot == 0) BestMove = move;
        }

        _transpositionTable.StoreEvaluation(plyRemaining, alpha, evaluationFlag);
        
        return alpha;
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
    
    private static bool FailHigh(int evaluation, int beta) => evaluation >= beta; // Cut-node
    private static bool FailLow(int evaluation, int alpha) => evaluation <= alpha; // All-node
}