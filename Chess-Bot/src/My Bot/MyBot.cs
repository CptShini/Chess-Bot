using ChessChallenge.API;
using ChessChallenge.Application;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer) => Settings.MyBot.Think(board, timer);
}