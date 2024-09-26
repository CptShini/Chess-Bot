using Chess_Challenge.My_Bot.CleverBot.CleverBotV2.Nodes;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV2;

public class CleverBotV2 : IChessBot
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
        bool newGameStarted = timer.MillisecondsRemaining == timer.GameStartTimeMilliseconds;
        if (newGameStarted) _nodeTree = new(board);
        
        _nodeTree.Update(board);
        return _nodeTree.GetMove().Move;
    }
}