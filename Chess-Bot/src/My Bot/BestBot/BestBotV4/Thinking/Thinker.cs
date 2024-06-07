using System.Diagnostics;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Thinking;

internal class Thinker
{
    private const int Infinity = 999999;
    
    private const int DepthHardLimit = 32;
    private const int ExpectedTurnCount = 30;
    
    private readonly long _maximumThinkTime;
    private readonly Searcher _searcher;
    private readonly Stopwatch _timer;

    internal Thinker(Board board, Timer timer)
    {
        _searcher = new(board);
        _timer = Stopwatch.StartNew();
        
        _maximumThinkTime = timer.MillisecondsRemaining / ExpectedTurnCount * Stopwatch.Frequency / 1000;
    }

    internal ScoredMove IterativeDeepening()
    {
        int alpha = -Infinity;
        int beta = Infinity;
        
        ScoredMove currentBest = Think(0, alpha, beta, GetThinkTimeEstimate(0), out long timeTaken);
        for (int depth = 1; depth < DepthHardLimit; depth++)
        {
            float thinkTimeEstimate = GetThinkTimeEstimate(depth, timeTaken);
            if (TimeToStopThinking(thinkTimeEstimate)) break;

            currentBest = Think(depth, alpha, beta, thinkTimeEstimate, out timeTaken);
            if (currentBest.IsCheckmate) break;
        }
        
        return currentBest;
    }
    
    private ScoredMove Think(int maxDepth, int alpha, int beta, float thinkTime, out long timeTaken)
    {
        Stopwatch s = Stopwatch.StartNew();
        
        //Task<int> thinkTask = Task.Run(() => _searcher.Search(ref line, maxDepth, alpha, beta));
        //if (!thinkTask.Wait(TimeSpan.FromMilliseconds(thinkTime * 1.5f))) throw new OperationCanceledException($"Time ran out while thinking after {s.ElapsedMilliseconds}ms.");
        
        Line line = new();
        int evaluation = _searcher.Search(ref line, maxDepth, alpha, beta);
        ScoredMove thoughtProduct = new(line, evaluation);
        
        timeTaken = s.ElapsedTicks;
        return thoughtProduct;
    }
    
    private bool TimeToStopThinking(float thinkTimeEstimate)
    {
        float estimatedEndTime = _timer.ElapsedTicks + thinkTimeEstimate;
        return estimatedEndTime > _maximumThinkTime;
    }
    
    private static float GetThinkTimeEstimate(int depth, long previousThinkTime = 0)
    {
        return depth switch
        {
            0 => previousThinkTime + 225f,
            1 => previousThinkTime * 3.7755f + 323.92f,
            2 => previousThinkTime * 14.321f - 1762.3f,
            3 => previousThinkTime * 8.6170f - 6272.1f,
            4 => previousThinkTime * 11.293f - 102303f,
            5 => previousThinkTime * 5.2390f + 232060f,
            6 => previousThinkTime * 7.4013f + 972526f,
            7 => previousThinkTime * 3.9426f + 20000000f,
            _ => previousThinkTime * 7f
        };
    }
    
    /*private static float GetThinkTimeEstimate(int depth, long previousThinkTime = 0)
    {
        return depth switch
        {
            0 => previousThinkTime + 10f,
            1 => previousThinkTime * 0.9902f + 2.0196f,
            2 => previousThinkTime * 0.7300f + 5.5400f,
            3 => previousThinkTime * 0.9394f + 12.532f,
            4 => previousThinkTime * 4.3539f + 19.885f,
            5 => previousThinkTime * 7.5979f + 23.850f,
            6 => previousThinkTime * 6.2155f + 414.05f,
            7 => previousThinkTime * 6.7331f - 91.745f,
            _ => previousThinkTime * 7f
        };
    }*/
}