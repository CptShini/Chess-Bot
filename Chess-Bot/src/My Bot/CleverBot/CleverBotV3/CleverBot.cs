using Chess_Challenge.My_Bot.CleverBot.CleverBotV3.Nodes;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV3;

public class CleverBot : IChessBot
{
    private NodeTree _nodeTree;

    /* TODO:
     * --- V3 ---
     * Time controlled search depth
     *
     * --- V4 ---
     * Alpha-Beta pruning search
     */
    public Move Think(Board board, Timer timer)
    {
        _nodeTree ??= new NodeTree(board, -1);
        //_nodeTree.Think(board, timer);

        _nodeTree.Update(board);

        return _nodeTree.GetMove();
    }
}