using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV3.Thinking;
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
            Endgame play, (How to evaluate?):
                King to King distance
                Enemy king near edges
            Transposition Table
            Move ordering (from Iterative Deepening)
        Improvements:
            Memory optimization using GetLegalMovesNonAlloc()
            Think-Time:
                Make these a part of the equation:
                    Current evaluation
                    Opponent remaining time
                    Endgame coefficient
                Neural Network model for estimating/predicting think time
        Problems/Bugs:
            Professional repetition enjoyer
    */

    private Thinker _thinker;
    
    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);

        ScoredMove scoredMove = _thinker.IterativeDeepening();
        Console.WriteLine(scoredMove);
        
        return scoredMove.Move;
    }
}