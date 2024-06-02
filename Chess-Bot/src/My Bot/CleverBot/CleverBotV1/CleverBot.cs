using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV1;

public class CleverBot : IChessBot
{
    private NodeTree _nodeTree;

    /* TODO:
     * Generalized traversal function
     * Keep track of high and low eval (no more linq, foreach, or looping in general)
     * Re-evaluation on subnote evaluation changed
     * Time controlled search depth
     *      Keep track of current best move, when timer done, return said move (potentially, maybe not)
     * Alpha-Beta pruning search
     * Improved evaluation function
     */
    public Move Think(Board board, Timer timer)
    {
        _nodeTree ??= new NodeTree(board);
        _nodeTree.Update(board);

        return _nodeTree.GetMove();
    }
}