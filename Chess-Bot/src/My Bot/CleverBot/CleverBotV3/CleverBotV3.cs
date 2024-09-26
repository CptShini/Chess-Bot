using Chess_Challenge.My_Bot.CleverBot.CleverBotV3.Nodes;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV3;

public class CleverBotV3 : IChessBot
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
        bool newGameStarted = timer.MillisecondsRemaining == timer.GameStartTimeMilliseconds;
        if (newGameStarted) _nodeTree = new(board);
        //_nodeTree.Think(board, timer);

        _nodeTree.Update(board);

        return _nodeTree.GetMove();
    }
}