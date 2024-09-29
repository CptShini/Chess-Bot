using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Thinking;

internal class Thinker
{
    //private const int Infinity = 999999;
    private const int DepthLimit = 64;
    private const int ExpectedTurnCount = 25;
    
    internal ScoredMove CurrentBest { get; private set; }
    
    private int depth;
    private long timeTaken;
    private readonly long _maximumTurnThinkTime;
    
    private readonly Searcher _searcher;
    private readonly Stopwatch _turnTimer;
    private static ThinkTimeEstimator _thinkTimeEstimator;

    internal Thinker(Board board, Timer timer)
    {
        int timeSinceGameStart = timer.GameStartTimeMilliseconds - timer.MillisecondsRemaining;
        bool newGameStarted = timeSinceGameStart < 50;
        if (newGameStarted) _thinkTimeEstimator = new();
        
        float thinkTimeMilliseconds = (float)timer.MillisecondsRemaining / ExpectedTurnCount;
        long tickMsConversion = Stopwatch.Frequency / 1000;
        _maximumTurnThinkTime = (long)(thinkTimeMilliseconds * tickMsConversion);
        
        _searcher = new(board);
        _turnTimer = Stopwatch.StartNew();
    }

    internal void IterativeDeepening()
    {
        while (depth < DepthLimit)
        {
            bool doneThinking = TryThink();
            if (!doneThinking || CurrentBest.IsCheckmate) break;
            
            if (TimeToStopThinking()) break;
            depth++;
        }
    }

    private bool TryThink()
    {
        Task task = Task.Factory.StartNew(Think);

        TimeSpan maximumThinkTime = TimeSpan.FromTicks((long)(_maximumTurnThinkTime * 1.5f));
        task.Wait(maximumThinkTime);
        
        return task.IsCompleted;
    }

    private void Think()
    {
        long previousTimeTaken = timeTaken;
        timeTaken = RunTakeTime(Search);
        
        _thinkTimeEstimator.AddBranch(depth, timeTaken, previousTimeTaken);
        
        return;

        static long RunTakeTime(Action action)
        {
            Stopwatch s = Stopwatch.StartNew();
            
            action();
            s.Stop();
            
            return s.ElapsedTicks;
        }
        
        void Search()
        {
            int evaluation = _searcher.Search(depth);
            CurrentBest = new(_searcher.BestMove, evaluation, depth);
        }
    }
    
    private bool TimeToStopThinking()
    {
        long thinkTimeEstimate = GetThinkTimeEstimate();

        long estimatedEndTime = _turnTimer.ElapsedTicks + thinkTimeEstimate;
        return estimatedEndTime > _maximumTurnThinkTime;
        
        long GetThinkTimeEstimate()
        {
            float branchFactor = _thinkTimeEstimator.GetAverageBranchFactor(depth + 1);
            return (long)(timeTaken * branchFactor);
        }
    }
}