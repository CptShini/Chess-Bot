using Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

public class BestBotV6 : IChessBot
{
    /*
     * TODO:
     * Add:
     *  - Handle draw positions
     *  - king safety
     *  - mop-up score (include in move ordering)
     * 
     * Fiddle/tinker with:
     *  - valueboards
     *
     * Improve/Rework:
     *  - think time calculator (more fiddling, endgame factor, enemy time remaining, etc.)
     *  - move ordering (+Killer moves)
     *
     * ==================== V7 ====================
     * Add:
     *  - Depth reduction
     * 
     * Refactor:
     *  - Searcher
     *  - Transposition table
     *
     * Retry:
     *  - not clearing transposition table?
     *  - aspiration windows
     * 
     * Add:
     *  - Passed Pawns
     *  - Isolated Pawns
     *
     * Research:
     *  - neural network evaluation (maybe)
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