using System;
using System.Threading;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV1.Evaluations;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV1;

public class BestBotV1 : IChessBot
{
    private readonly Random _rnd = new();
    private Evaluation _evaluation;

    private const int ExpectedTurnCount = 60;
    private const int EmergencyTurnCount = 20;

    public Move Think(Board board, Timer timer)
    {
        _evaluation = new(board);

        return GetMove(board, timer);
    }

    Move GetMove(Board board, Timer timer)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        Task<(float eval, Move move, int depth)> task = Task.Run(() => IterativeDeepning(board, cts.Token));

        TimeSpan thinkTime = CalculateThinkTime(timer);
        if (!task.Wait(thinkTime)) cts.Cancel();

        this.PrintMove(task.Result.move, task.Result.depth, task.Result.eval, 90f);

        return task.Result.move;
    }

    static TimeSpan CalculateThinkTime(Timer timer)
    {
        // TODO: Factor in current evaluation (whether we are ahead or not)
        // TODO: Potentially factor in enemy remaining time?
        // TODO: Base entire time algorithm off some endgame coefficient

        int startTime = timer.GameStartTimeMilliseconds;
        int turnCountFraction = startTime / ExpectedTurnCount;

        int remainingTime = timer.MillisecondsRemaining;
        int remainingFraction = remainingTime / EmergencyTurnCount;

        int timeThisTurn = Math.Min(turnCountFraction, remainingFraction);
        return TimeSpan.FromMilliseconds(timeThisTurn);
    }

    (float, Move, int) IterativeDeepning(Board board, CancellationToken ct)
    {
        float bestEval = 0f;
        Move bestMove = Move.NullMove;

        int depth = 0;
        for (;; depth++)
        {
            (float eval, Move move) = EvaluateMoves(board, depth, ct);

            // TODO: Move ordering by remembering best moves from previous search.

            if (ct.IsCancellationRequested) break;

            bestEval = eval;
            bestMove = move;

            if (Math.Abs(bestEval) > 99f) break;
        }

        return (bestEval, bestMove, depth);
    }

    (float, Move) EvaluateMoves(Board board, int maxDepth, CancellationToken ct, int depth = 0,
        float alpha = int.MinValue, float beta = int.MaxValue)
    {
        float bestEval = 0f;
        Move bestMove = Move.NullMove;
        bool isMax = board.IsWhiteToMove;
        bool depthReached = depth == maxDepth;

        Move[] moves = board.GetLegalMoves();
        foreach (Move move in moves)
        {
            if (ct.IsCancellationRequested) break;

            _evaluation.MakeMove(move);
            float eval = EvaluateMove();
            _evaluation.UndoMove(move);

            if (isMax ? eval > beta : eval < alpha) return (eval, Move.NullMove);

            if ((isMax ? eval > bestEval : eval < bestEval) || bestMove == Move.NullMove ||
                eval == bestEval && _rnd.Next(2) == 0)
            {
                bestMove = move;
                bestEval = eval;
            }
        }

        return (bestEval, bestMove);

        float EvaluateMove()
        {
            bool searchFinished = depthReached || _evaluation.GameHasEnded;
            if (searchFinished) return _evaluation.Current;

            if (bestMove != Move.NullMove)
            {
                if (isMax) alpha = Math.Max(alpha, bestEval);
                else beta = Math.Min(beta, bestEval);
            }

            return EvaluateMoves(board, maxDepth, ct, depth + 1, alpha, beta).Item1;
        }
    }
}