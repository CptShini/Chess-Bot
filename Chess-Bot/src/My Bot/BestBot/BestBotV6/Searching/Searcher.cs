using System;
using System.Threading;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Transpositions;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Transpositions.TranspositionTable;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching;

internal class Searcher
{
    private const int Infinity = 9999_99;
    
    private readonly BoardEvaluation _boardEvaluation;
    private static readonly TranspositionTable _transpositionTable = new(TTSize);

    private CancellationToken _ct;
    
    internal Move BestMove { get; private set; }

    internal Searcher(Board board)
    {
        _boardEvaluation = new(board);
        _transpositionTable.Initialize(board);

        BestMove = Move.NullMove;
    }

    internal int Search(int depth, CancellationToken ct, int alpha = -Infinity, int beta = Infinity)
    {
        _ct = ct;
        return Search(depth, 0, alpha, beta);
    }

    private int Search(int plyRemaining, int plyFromRoot, int alpha, int beta, int numExtensions = 0)
    {
        if (_ct.IsCancellationRequested) return 0;
        
        if (plyFromRoot > 0)
        {
            GameState gameState = _boardEvaluation.EvaluateGameState(plyFromRoot, out int endEvaluation);
            if (gameState != GameState.GameNotOver) return endEvaluation;
        
            int ttVal = _transpositionTable.LookupEvaluation(plyRemaining, alpha, beta);
            if (ttVal != LookupFailed) return ttVal;
        }
        
        bool depthReached = plyRemaining == 0;
        if (depthReached) return QuiescentSearch(alpha, beta);
        
        TranspositionFlag evaluationFlag = TranspositionFlag.Alpha;
        Move bestMoveThisPosition = Move.NullMove;
        Move pvMove = plyFromRoot == 0 ? BestMove : _transpositionTable.TryGetStoredMove();
        
        Span<Move> moves = stackalloc Move[128];
        _boardEvaluation.GetOrderedMoves(ref moves, false, pvMove);
        
        foreach (Move move in moves)
        {
            if (move.IsNull) break;
            
            _boardEvaluation.MakeMove(move);

            int extension = GetExtension(numExtensions, move);
            int evaluation = -Search(plyRemaining - 1 + extension, plyFromRoot + 1, -beta, -alpha, numExtensions + extension);
            _boardEvaluation.UndoMove(move);
            
            if (_ct.IsCancellationRequested) return 0;

            if (FailHigh(evaluation, beta)) // Prune
            {
                _transpositionTable.StoreEvaluation(plyRemaining, beta, TranspositionFlag.Beta, move);
                return beta;
            }
            if (FailLow(evaluation, alpha)) continue; // Ignore
            
            // PV-node
            alpha = evaluation;
            evaluationFlag = TranspositionFlag.Exact;
            bestMoveThisPosition = move;
            if (plyFromRoot == 0) BestMove = move;
        }

        _transpositionTable.StoreEvaluation(plyRemaining, alpha, evaluationFlag, bestMoveThisPosition);
        
        return alpha;
    }
    
    private int GetExtension(int numExtensions, Move move)
    {
        bool tooManyExtensions = numExtensions >= MaxExtensions;
        if (tooManyExtensions) return 0;

        bool moveIsCheck = _boardEvaluation.IsInCheck();
        if (moveIsCheck) return 1;

        bool closeToPromotion = move is { MovePieceType: PieceType.Pawn, TargetSquare.Rank: 1 or 6 };
        return closeToPromotion ? 1 : 0;
    }
    
    private int QuiescentSearch(int alpha, int beta)
    {
        if (_ct.IsCancellationRequested) return 0;
        
        int evaluationCurrent = _boardEvaluation.Current;
        if (FailHigh(evaluationCurrent, beta)) return beta; // Prune
        if (!FailLow(evaluationCurrent, alpha)) alpha = evaluationCurrent;
        
        Span<Move> moves = stackalloc Move[128];
        _boardEvaluation.GetOrderedMoves(ref moves, true, Move.NullMove);
        
        foreach (Move move in moves)
        {
            if (move.IsNull) break;
            
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
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("=== Searcher State ===");
        sb.AppendLine($"Best {BestMove}");
        sb.AppendLine();

        sb.AppendLine("--- Board Evaluation ---");
        sb.AppendLine(_boardEvaluation.ToString());

        sb.AppendLine("--- Transposition Table ---");
        sb.AppendLine(_transpositionTable.ToString());

        return sb.ToString();
    }
}