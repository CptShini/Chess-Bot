using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal class Searcher
{
    private BoardEvaluation _boardEvaluation;
    private static readonly TranspositionTable _transpositionTable = new(16);
    
    internal Move BestMove { get; private set; }

    internal Searcher(Board board)
    {
        _boardEvaluation = new(board);
        _transpositionTable.Initialize(board);
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
        Move bestMoveThisPosition = Move.NullMove;
        
        Span<Move> moves = stackalloc Move[128];
        Move pvMove = plyFromRoot == 0 ? BestMove : _transpositionTable.TryGetStoredMove();
        _boardEvaluation.FillOrderedMoves(ref moves, pvMove, false);
        
        foreach (Move move in moves)
        {
            if (move == Move.NullMove) break;
            
            _boardEvaluation.MakeMove(move);
            int evaluation = -Search(plyRemaining - 1, plyFromRoot + 1, -beta, -alpha);
            _boardEvaluation.UndoMove(move);

            if (FailHigh(evaluation, beta)) // Prune
            {
                _transpositionTable.StoreEvaluation(plyRemaining, beta, TranspositionTable.FlagBeta, move);
                return beta;
            }
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
            evaluationFlag = TranspositionTable.FlagExact;
            bestMoveThisPosition = move;
            if (plyFromRoot == 0) BestMove = move;
        }

        _transpositionTable.StoreEvaluation(plyRemaining, alpha, evaluationFlag, bestMoveThisPosition);
        
        return alpha;
    }

    private int QuiescentSearch(int alpha, int beta)
    {
        int evaluationCurrent = _boardEvaluation.Current;
        if (FailHigh(evaluationCurrent, beta)) return beta; // Prune
        if (!FailLow(evaluationCurrent, alpha)) alpha = evaluationCurrent;
        
        Span<Move> moves = stackalloc Move[128];
        _boardEvaluation.FillOrderedMoves(ref moves, Move.NullMove, true);
        
        foreach (Move move in moves)
        {
            if (move == Move.NullMove) break;
            
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