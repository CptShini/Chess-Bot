using System;
using System.Threading;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV5.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5;

public class BestBotV5 : IChessBot
{
    /*
    TODO:
        Aspiration Windows
        Better move ordering
    */

    private Thinker _thinker;

    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        
        ScoredMove scoredMove = _thinker.IterativeDeepening();
        Console.WriteLine(scoredMove);

        if (scoredMove.Move != Move.NullMove) return scoredMove.Move;
        
        Console.WriteLine("Nah, it's gg");
        Thread.Sleep(timer.MillisecondsRemaining);
        
        return Move.NullMove;
    }
}