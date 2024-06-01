using ChessChallenge.API;
using ChessChallenge.Application;

namespace Chess_Challenge.src.Evil_Bot;

public class OpponentBot : IChessBot
{
    public Move Think(Board board, Timer timer) => Settings.OpponentBot.Think(board, timer);
}