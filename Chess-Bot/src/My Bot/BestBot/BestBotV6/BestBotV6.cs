using Chess_Challenge.My_Bot.BestBot.BestBotV6.Thinking;
using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

public class BestBotV6 : IChessBot
{
    /*
     * TODO:
     * Fen strings to analyze:
     * - Weird null move:
     *     * r4rk1/pp1nqpp1/2pbpn1p/3p1b2/2PP4/1PN1PN2/PB2BPPP/R2QR1K1 w - - 6 11
     *  - why no queen promotion?
     *     * 5r1k/rpp3R1/1b2BB2/p4pP1/3P4/4P3/P4PK1/8 b - - 0 34
     *     * 6R1/rpp2P1k/1b2B3/p7/3P1p2/4P3/P4PK1/8 b - - 0 37
     *
     * Complete Systems analysis...
     * 
     * Fiddle/tinker with:
     *  - turn think time
     *  - piece values
     *  - state & move values
     *  - InsertionSortThreshold (try 8, 12, 16, and 20)
     *  - valueboards
     * 
     * Add:
     *  - dynamic transposition table size?
     *  - reward for king approaching enemy king in endgame
     *  - king safety
     *
     * Improve/refactor:
     *  - searcher
     *  - think time calculator (more fiddling, endgame factor, enemy time remaining, etc.)
     *  - move ordering
     *
     * Retry:
     *  - aspiration windows
     *  - alpha-beta window tightening
     *  - move ordering with iterative deepening and transposition tables
     *
     * Research:
     *  - neural network evaluation (maybe)
     *  - 2nd video features (since everything up until now was basically video 1)
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