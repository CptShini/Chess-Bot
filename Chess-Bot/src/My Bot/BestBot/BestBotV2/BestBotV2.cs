using Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations;
using ChessChallenge.API;
using System;
using System.Threading.Tasks;
using System.Threading;
using Timer = ChessChallenge.API.Timer;
using System.Collections.Generic;
using Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2;

public class BestBotV2 : IChessBot
{
    /*
    TODO:

    Random stuff
        Memory optimization using GetLegalMovesNonAlloc()
        Move ordering by remembering best moves from previous search
        Count number of attackers and defenders on each piece, maybe on depth reached, we continuing searching using a trade-search method
        Better endgame play :/
        Never recalculate already evaluated positions
        Make EvaluateMoves() only work with the evaluation, and not the bestmove (basically just separate the two)
            Switch to using only alpha and beta
    Think-time
        Factor in current evaluation (whether we are ahead or not)
        Potentially factor in enemy remaining time?
        Base entire time algorithm off some endgame/progression coefficient
        Algorithm for determining whether or not calculating an extra layer would take too long
    Evaluations
        Improve move guess evaluation
            Also including positioning?
        Evaluate King Safety
        Initial board evaluation, evaluating postioning as well
        Positioning
            Reward developing pieces
                Moving them from initial position
                Not moving the same piece several times in a row (twice?)
            Attacking/Defending
                Counting and evaluating how many pieces are defending and attacking given squares? (Probably useless)
            Valueboards
                More fine-tuning
                More pieces with valueboards? (King)
                Progression Coefficient

    */

    private readonly Random _rnd = new();
    private Evaluation _evaluation;
    private CancellationTokenSource _cts;

    private const int ExpectedTurnCount = 70;
    private const float SpeedUpTime = 4;

    public Move Think(Board board, Timer timer)
    {
        _cts = new CancellationTokenSource();
        _evaluation = new(board);

        //EvaluateMoves(board, 0);
        //EvaluateMoves(board, 1);
        //EvaluateMoves(board, 2);
        //EvaluateMoves(board, 3);
        //EvaluateMoves(board, 4);
        //EvaluateMoves(board, 5);

        return GetMove(board, timer);
    }

    Move GetMove(Board board, Timer timer)
    {
        Task<(float eval, Move move, int moveDepth)> task = Task.Run(() => IterativeDeepning(board));
        
        TimeSpan thinkTime = CalculateThinkTime(timer);
        if (!task.Wait(thinkTime)) _cts.Cancel();

        Console.WriteLine($"{task.Result.move} | Move-Depth: {task.Result.moveDepth} | Eval: {task.Result.eval:0.00}");

        return task.Result.move;
    }

    static TimeSpan CalculateThinkTime(Timer timer)
    {
        int startTime = timer.GameStartTimeMilliseconds;
        int turnCountFraction = startTime / ExpectedTurnCount;

        int remainingTime = timer.MillisecondsRemaining;
        int remainingFraction = (int)(remainingTime / (ExpectedTurnCount / SpeedUpTime));

        int timeThisTurn = Math.Min(turnCountFraction, remainingFraction);
        return TimeSpan.FromMilliseconds(timeThisTurn);
    }

    (float, Move, int) IterativeDeepning(Board board)
    {
        float bestEval = 0f;
        Move bestMove = Move.NullMove;

        int depth = 1;
        for (; ; depth += 2)
        {
            (float eval, Move move) = EvaluateMoves(board, depth, 0);

            if (_cts.Token.IsCancellationRequested) break;

            bestEval = eval;
            bestMove = move;

            if (Math.Abs(bestEval) > 99f) break;
        }

        return (bestEval, bestMove, (depth - 1) / 2);
    }

    (float, Move) EvaluateMoves(Board board, int maxDepth, int depth = 0, float alpha = int.MinValue, float beta = int.MaxValue)
    {
        float bestEval = 0f;
        Move bestMove = Move.NullMove;
        bool depthReached = depth == maxDepth;

        Move[] legalMoves = board.GetLegalMoves();
        IEnumerable<Move> moves = depthReached ? legalMoves : legalMoves.GuessOrder(board);
        foreach (Move move in moves)
        {
            if (_cts.Token.IsCancellationRequested) break;

            _evaluation.MakeMove(move);
            float eval = EvaluateMove();
            _evaluation.UndoMove(move);

            if (_evaluation.IsMax ? eval > beta : eval < alpha) return (eval, Move.NullMove);

            UpdateBestMove(eval, move);
        }

        return (bestEval, bestMove);

        float EvaluateMove()
        {
            if (_evaluation.GameHasEnded) return _evaluation.Current;

            bool searchFinished = depthReached;
            if (searchFinished) return EvaluateTrades(board);

            if (bestMove != Move.NullMove)
            {
                if (!_evaluation.IsMax) alpha = Math.Max(alpha, bestEval);
                else beta = Math.Min(beta, bestEval);
            }

            return EvaluateMoves(board, maxDepth, depth + 1, alpha, beta).Item1;
        }

        void UpdateBestMove(float eval, Move move)
        {
            bool noBestMove = bestMove == Move.NullMove;
            bool newMoveIsBetter = _evaluation.IsMax ? eval > bestEval : eval < bestEval;
            bool newMoveIsEqual = eval == bestEval && _rnd.Next(2) == 0;

            if (newMoveIsBetter || noBestMove || newMoveIsEqual)
            {
                bestMove = move;
                bestEval = eval;
            }
        }
    }

    float EvaluateTrades(Board board)
    {
        float bestEval = _evaluation.Current;

        Move[] captureMoves = board.GetLegalMoves(true);
        foreach (Move captureMove in captureMoves.GuessOrder(board))
        {
            /*float eval = _evaluation.EvaluateMove(captureMove, () => EvaluateTrades(board));

            bool newMoveIsBetter = board.IsWhiteToMove ? eval > bestEval : eval < bestEval;
            bool newMoveIsEqual = eval == bestEval && _rnd.Next(2) == 0;
            if (newMoveIsBetter || newMoveIsEqual) bestEval = eval;*/
        }

        return bestEval;
    }
}