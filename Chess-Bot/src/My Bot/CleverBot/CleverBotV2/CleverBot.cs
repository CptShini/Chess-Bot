using ChessChallenge.API;
using CleverBot.CleverBotV2.Nodes;

namespace CleverBot.CleverBotV2;

public class CleverBot : IChessBot
{
    private NodeTree _nodeTree;

    /* TODO:
     * --- V3 ---
     * Time controlled search depth
     *      Diagnostics showing how deep it's thinking
     *          Evaluation depth (number which describes from how far down, said evaluation, comes from)
     *      
     * --- V4 ---
     * Alpha-Beta pruning search
     */
    public Move Think(Board board, Timer timer)
    {
        _nodeTree ??= new NodeTree(board);
        _nodeTree.Update(board);

        return _nodeTree.GetMove().Move;
    }
}