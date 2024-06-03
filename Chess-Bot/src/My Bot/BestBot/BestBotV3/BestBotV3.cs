using System;
using System.Threading;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3;

public class BestBotV3 : IChessBot
{
    /*
    TODO:
        Non-bot Stuff:
            Running several games at once?
        Features:
            Endgame play:
                King to King distance
                Enemy king near edges
            Transposition Table
            Move ordering (from Iterative Deepening)
        Improvements:
            Memory optimization using GetLegalMovesNonAlloc()
            Think-Time:
                Factor in current evaluation (whether we are ahead or not)
                Potentially factor in enemy remaining time?
                Base entire time algorithm off some endgame/progression coefficient
                Algorithm for determining/just precalculating whether or not searching an extra layer would take too long
        Problems/Bugs:
            Professional repetition enjoyer
            Still potentially some checkmate/draw bugs?
            Promoting pawns to stuff other than queens?
                Probably caused when it'll be checkmate either way?
    */

    private static readonly Random Random = new();
    
    private BoardEvaluation _boardEvaluation;
    private CancellationTokenSource _cts;

    private const int ExpectedTurnCount = 70;
    private const float SpeedUpTime = 4;
    
    public Move Think(Board board, Timer timer)
    {
        _cts = new();
        _boardEvaluation = new(board);

        return GetMove(board, timer);
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
    
    private Move GetMove(Board board, Timer timer)
    {
        Task<(int eval, Move move, int moveDepth)> task = Task.Run(() => IterativeDeepening(board));

        TimeSpan thinkTime = CalculateThinkTime(timer);
        if (!task.Wait(thinkTime)) _cts.Cancel();

        Console.WriteLine($"{task.Result.move} | Move-Depth: {task.Result.moveDepth} | Eval: {task.Result.eval}");

        return task.Result.move;
    }
    
    private (int, Move, int) IterativeDeepening(Board board)
    {
        int bestEvaluation = 0;
        Move bestMove = board.GetLegalMoves()[0];

        int depth = 1;
        for (;; depth++)
        {
            var (eval, move) = SearchForBestMove(board, depth);
            
            if (_cts.Token.IsCancellationRequested) break;
            
            bestEvaluation = eval;
            bestMove = move;

            if (bestEvaluation >= 9000) break;
        }

        return (bestEvaluation, bestMove, depth - 1);
    }
    
    private (int, Move) SearchForBestMove(Board board, int maxDepth)
    {
        Move bestMove = Move.NullMove;
        int bestEvaluation = int.MinValue;

        Move[] legalMoves = board.GetLegalMoves();
        foreach (Move move in legalMoves)
        {
            int moveEvaluation = _boardEvaluation.EvaluateMove(move, () => -Search(maxDepth));
            
            if (_cts.Token.IsCancellationRequested) break;
            
            bool newMoveIsBetter = moveEvaluation > bestEvaluation;
            bool sameMoveButFunnier = moveEvaluation == bestEvaluation && Random.Next(2) == 0;
            if (!(newMoveIsBetter || sameMoveButFunnier)) continue;
            
            bestEvaluation = moveEvaluation;
            bestMove = move;
        }
        
        return (bestEvaluation, bestMove);
    }

    private int Search(int depth, int alpha = -999999, int beta = 999999)
    {
        if (depth == 0)
        {
            bool gameHasEnded = _boardEvaluation.GameHasEnded(out int endEvaluation);
            return gameHasEnded ? endEvaluation : SearchAllCaptures(alpha, beta);
        }

        if (_cts.Token.IsCancellationRequested) return alpha;
        
        return _boardEvaluation.AlphaBetaEvaluateMoves(() => -Search(depth - 1, -beta, -alpha), ref alpha, beta);
    }

    private int SearchAllCaptures(int alpha, int beta)
    {
        int evaluationCurrent = _boardEvaluation.Current;

        if (evaluationCurrent >= beta) return beta;
        if (evaluationCurrent > alpha) alpha = evaluationCurrent;

        if (_cts.Token.IsCancellationRequested) return alpha;
        
        return _boardEvaluation.AlphaBetaEvaluateMoves(() => -SearchAllCaptures(-beta, -alpha), ref alpha, beta, true);
    }
}