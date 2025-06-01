using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;

internal class Thinker
{
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
        
        float thinkTimeMilliseconds = TurnThinkTime(timer.MillisecondsRemaining);
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
            if (!doneThinking || CurrentBest.GameState != GameState.GameNotOver) break;
            
            if (TimeToStopThinking()) break;
            depth++;
        }
    }

    private bool TryThink()
    {
        Task thinkTask = Task.Factory.StartNew(Think);

        TimeSpan maximumThinkTime = TimeSpan.FromTicks((long)(_maximumTurnThinkTime * MaxThinkTimeFactor));
        thinkTask.Wait(maximumThinkTime);
        
        return thinkTask.IsCompleted;
        
        void Think()
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
                int evaluation = _searcher.Search(depth, out GameState gameState);
                CurrentBest = new(_searcher.BestMove, evaluation, depth, gameState);
            }
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