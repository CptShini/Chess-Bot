using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations;
using Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2;

public class BestBotV2 : IChessBot
{
    /*
    TODO:

    Priority Stuff
        Transposition Tables for speed
            Evaluate actual speed/performance impact
        Do some performance profiling (it's probably the trade evaluation method)
            Find something better than the in-built profiler
        Potentially remove checkmate/draw check in trade evaluation method
            Evaluate actual speed/performance impact
        Figure out how to set up specific board position and how to run several games at once
    Random Stuff
        Memory optimization using GetLegalMovesNonAlloc()
        Move ordering by remembering best moves from previous search
        Count number of attackers and defenders on each piece, maybe on depth reached, we continuing searching using a trade-search method
        Make EvaluateMoves() only work with the evaluation, and not the bestmove (basically just separate the two)
            Switch to using only alpha and beta
    Think-time
        Factor in current evaluation (whether we are ahead or not)
        Potentially factor in enemy remaining time?
        Base entire time algorithm off some endgame/progression coefficient
        Algorithm for determining whether or not calculating an extra layer would take too long
    Evaluations
        Evaluate King Safety
        Positioning
            Reward developing pieces
                Moving them from initial position
                Not moving the same piece several times in a row (twice?)
            Attacking/Defending
                Counting and evaluating how many pieces are defending and attacking given squares? (Probably useless)
    */

    private Evaluation _evaluation;
    private CancellationTokenSource _cts;

    private const int ExpectedTurnCount = 70;
    private const float SpeedUpTime = 4;

    public Move Think(Board board, Timer timer)
    {
        _cts = new();
        _evaluation = new(board);

        return GetMove(board, timer);
    }

    private Move GetMove(Board board, Timer timer)
    {
        Task<(float eval, Move move, int moveDepth)> task = Task.Run(() => IterativeDeepening(board));

        TimeSpan thinkTime = CalculateThinkTime(timer);
        if (!task.Wait(thinkTime)) _cts.Cancel();
        
        Console.WriteLine($"BestBotV2: {task.Result.move} | Depth: {task.Result.moveDepth} | Evaluation: {task.Result.eval:0.00}");

        return task.Result.move;
    }

    private static TimeSpan CalculateThinkTime(Timer timer)
    {
        int startTime = timer.GameStartTimeMilliseconds;
        int turnCountFraction = startTime / ExpectedTurnCount;

        int remainingTime = timer.MillisecondsRemaining;
        int remainingFraction = (int)(remainingTime / (ExpectedTurnCount / SpeedUpTime));

        int timeThisTurn = Math.Min(turnCountFraction, remainingFraction);
        return TimeSpan.FromMilliseconds(timeThisTurn);
    }

    private (float, Move, int) IterativeDeepening(Board board)
    {
        float bestEval = 0f;
        Move bestMove = board.GetLegalMoves()[0];

        int depth = 1;
        for (;; depth += 2)
        {
            (float eval, Move move) = EvaluateMoves(board, depth);

            if (_cts.Token.IsCancellationRequested) break;

            bestEval = eval;
            bestMove = move;

            if (Math.Abs(bestEval) > 99f) break;
        }

        return (bestEval, bestMove, (depth - 1) / 2);
    }

    private (float, Move) EvaluateMoves(Board board, int maxDepth, int depth = 0, float alpha = int.MinValue, float beta = int.MaxValue)
    {
        float bestEval = board.IsWhiteToMove ? int.MinValue : int.MaxValue;
        Move bestMove = Move.NullMove;
        bool depthReached = depth == maxDepth;

        Move[] legalMoves = board.GetLegalMoves();
        IEnumerable<Move> moves = depthReached ? legalMoves : legalMoves.GuessOrder(board);
        foreach (Move move in moves)
        {
            if (_cts.Token.IsCancellationRequested) break;

            _evaluation.MakeMove(move);
            float eval = EvaluateMove(depthReached, AtDepthEvaluationFunction, AlphaBetaUpdater, RecursiveEvaluationFunction);
            _evaluation.UndoMove(move);

            if (board.IsWhiteToMove ? eval > beta : eval < alpha) return (eval, Move.NullMove);

            bool newMoveIsBetter = board.IsWhiteToMove ? eval > bestEval : eval < bestEval;
            if (!newMoveIsBetter) continue;
            
            bestMove = move;
            bestEval = eval;
        }

        return (bestEval, bestMove);
        
        float AtDepthEvaluationFunction() => EvaluateTrades(board);
        void AlphaBetaUpdater() => UpdateAlphaBeta(board, ref alpha, ref beta, bestEval);
        float RecursiveEvaluationFunction() => EvaluateMoves(board, maxDepth, depth + 1, alpha, beta).Item1;
    }

    private float EvaluateMove(bool depthReached, Func<float> atDepthEvaluationFunction, Action alphaBetaUpdater, Func<float> recursiveEvaluationFunction)
    {
        if (_evaluation.GameHasEnded) return _evaluation.Current;
        if (depthReached) return atDepthEvaluationFunction();

        alphaBetaUpdater();

        return recursiveEvaluationFunction();
    }

    private static void UpdateAlphaBeta(Board board, ref float alpha, ref float beta, float bestEval)
    {
        if (!board.IsWhiteToMove) alpha = Math.Max(alpha, bestEval);
        else beta = Math.Min(beta, bestEval);
    }

    private float EvaluateTrades(Board board, float alpha = int.MinValue, float beta = int.MaxValue)
    {
        Move[] captureMoves = board.GetLegalMoves(true);
        if (captureMoves.Length == 0 || _evaluation.GameHasEnded) return _evaluation.Current;

        alpha = beta = _evaluation.Current;

        bool isMax = board.IsWhiteToMove;
        foreach (Move captureMove in captureMoves)
        {
            if (_cts.Token.IsCancellationRequested) break;

            _evaluation.MakeMove(captureMove, true);
            float eval = EvaluateTrades(board, alpha, beta);
            _evaluation.UndoMove(captureMove);

            switch (isMax)
            {
                case true when eval > alpha:
                    alpha = eval;
                    break;
                case false when eval < beta:
                    beta = eval;
                    break;
            }

            if (isMax ? eval > beta : eval < alpha) return eval;
        }

        return isMax ? alpha : beta;
    }
}