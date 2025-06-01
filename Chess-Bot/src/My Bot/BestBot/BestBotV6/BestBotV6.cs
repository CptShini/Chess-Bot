using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

public class BestBotV6 : IChessBot
{
    /*
     * TODO:
     *
     * Revert back to previous version...
     *
     * TTE unknown (+1) depth looks at depth -2?
     * 
     * Add or improve:
     *  - move ordering
     *  - king safety
     *  - think time calculator (more fiddling, endgame factor, enemy time remaining, etc.)
     * 
     * Retry Aspiration windows
     * Reexamine move ordering with iterative deepening and transposition tables
     * Retry alpha-beta window tightening
     *
     * Neural network evaluation (maybe)
     *
     * Research 2nd video features (since everything up until now was basically video 1)
     */
    
    private Thinker _thinker;
    
    public Move Think(Board board, Timer timer)
    {
        bool isWhiteToMove = board.IsWhiteToMove;
        _thinker = new(board, timer);
        _thinker.IterativeDeepening();
        
        ScoredMove currentBest = _thinker.CurrentBest;
        this.PrintMove(isWhiteToMove, currentBest);
        
        return currentBest.Move;
    }
}