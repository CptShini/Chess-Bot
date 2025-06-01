using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;
using Chess_Challenge.My_Bot.BestBot.BestBotV4.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4;

public class BestBotV4 : IChessBot
{
    /*
    TODO:
        PVS
        Iterative Deepening
            Move Ordering
            Aspiration Windows
        Transposition Table
    */

    private Thinker _thinker;

    public Move Think(Board board, Timer timer)
    {
        bool isWhiteToMove = board.IsWhiteToMove;
        _thinker = new(board, timer);
        
        ScoredMove scoredMove = _thinker.IterativeDeepening();
        this.PrintMove(isWhiteToMove, scoredMove);
        
        return scoredMove.Move;
    }
}