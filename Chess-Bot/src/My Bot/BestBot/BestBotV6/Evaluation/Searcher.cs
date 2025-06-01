using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions.TranspositionTable;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;

internal class Searcher
{
    private const int Infinity = 9999_99;
    
    private BoardEvaluation _boardEvaluation;
    private static readonly TranspositionTable _transpositionTable = new(TTSize);
    
    internal Move BestMove { get; private set; }

    internal Searcher(Board board)
    {
        _boardEvaluation = new(board);
        _transpositionTable.Initialize(board);

        BestMove = board.GetLegalMoves().RandomElement();
    }

    internal int Search(int plyRemaining, out GameState gameState, int plyFromRoot = 0, int alpha = -Infinity, int beta = Infinity)
    {
        if (plyFromRoot > 0)
        {
            gameState = _boardEvaluation.CheckGameState(plyFromRoot, out int endEvaluation);
            if (gameState != GameState.GameNotOver) return endEvaluation;
        
            int ttVal = _transpositionTable.LookupEvaluation(plyRemaining, alpha, beta);
            if (ttVal != LookupFailed) return ttVal;
        } else gameState = GameState.GameNotOver;
        
        bool depthReached = plyRemaining == 0;
        return !depthReached ? SearchMoves(ref gameState) : QuiescentSearch(alpha, beta);

        int SearchMoves(ref GameState pvGameState)
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
                int evaluation = -Search(plyRemaining - 1, out GameState moveState, plyFromRoot + 1, -beta, -alpha);
                _boardEvaluation.UndoMove(move);

                if (FailHigh(evaluation, beta)) // Prune
                {
                    _transpositionTable.StoreEvaluation(plyRemaining, beta, FlagBeta, move);
                    return beta;
                }
                if (FailLow(evaluation, alpha)) continue; // Ignore
            
                // PV-node
                alpha = evaluation;
                pvGameState = moveState;
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