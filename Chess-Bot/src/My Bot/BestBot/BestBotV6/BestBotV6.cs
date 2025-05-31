using Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

public class BestBotV6 : IChessBot
{
    /*
     * TODO:
     *
     * Fix random opposite sign??? (and rarely, checkmate evaluation???)
     * 
     * Experiment with
     *  - all constants
     *  - valueboards
     *
     * Better
     *  - move ordering
     *  - king safety
     * 
     * Retry Aspiration windows
     * Reexamine move ordering with iterative deepening and transposition tables
     * Retry alpha-beta window tightening
     *
     * Research 2nd video features (since everything up until now was basically video 1)
     */
    
    private Thinker _thinker;
    
    public Move Think(Board board, Timer timer)
    {
        _thinker = new(board, timer);
        _thinker.IterativeDeepening();
        
        ScoredMove currentBest = _thinker.CurrentBest;
        this.PrintMove(board, currentBest);
        
        return currentBest.Move;
    }
}