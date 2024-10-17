using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions.TranspositionTable;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;

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
        if (plyFromRoot > 0)
        {
            alpha = Math.Max(alpha, -10000 + plyFromRoot);
            beta = Math.Min(beta, 10000 - plyFromRoot);
            if (FailHigh(alpha, beta)) return alpha;
        }
        
        bool gameHasEnded = _boardEvaluation.GameHasEnded(out int endEvaluation);
        if (gameHasEnded) return endEvaluation;
        
        int ttVal = _transpositionTable.LookupEvaluation(plyRemaining, alpha, beta);
        if (ttVal != LookupFailed) return ttVal;
        
        bool depthReached = plyRemaining == 0;
        return !depthReached ? SearchMoves() : QuiescentSearch(alpha, beta);
        
        int SearchMoves()
        {
            int evaluationFlag = FlagAlpha;
            Move bestMoveThisPosition = Move.NullMove;
            Move pvMove = plyFromRoot == 0 ? BestMove : _transpositionTable.TryGetStoredMove();
        
            Span<Move> moves = stackalloc Move[128];
            _boardEvaluation.PopulateMoves(ref moves, false);
            _boardEvaluation.OrderMoves(ref moves, pvMove);
        
            foreach (Move move in moves)
            {
                if (move == Move.NullMove) break;
            
                _boardEvaluation.MakeMove(move);
                int evaluation = -Search(plyRemaining - 1, plyFromRoot + 1, -beta, -alpha);
                _boardEvaluation.UndoMove(move);

                if (FailHigh(evaluation, beta)) // Prune
                {
                    _transpositionTable.StoreEvaluation(plyRemaining, beta, FlagBeta, move);
                    return beta;
                }
                if (FailLow(evaluation, alpha)) continue; // Ignore
            
                // PV-node
                alpha = evaluation;
                evaluationFlag = FlagExact;
                bestMoveThisPosition = move;
                if (plyFromRoot == 0) BestMove = move;
            }

            _transpositionTable.StoreEvaluation(plyRemaining, alpha, evaluationFlag, bestMoveThisPosition);
        
            return alpha;
        }
    }
    
    private int QuiescentSearch(int alpha, int beta)
    {
        int evaluationCurrent = _boardEvaluation.Current;
        if (FailHigh(evaluationCurrent, beta)) return beta; // Prune
        if (!FailLow(evaluationCurrent, alpha)) alpha = evaluationCurrent;
        
        Span<Move> moves = stackalloc Move[128];
        _boardEvaluation.PopulateMoves(ref moves, true);
        _boardEvaluation.OrderMoves(ref moves, Move.NullMove);
        
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