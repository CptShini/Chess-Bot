using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

internal readonly struct ScoredMove
{
    internal readonly Move Move;
    
    internal readonly int Evaluation;
    internal readonly int Depth;
    internal readonly GameState GameState;
    
    internal ScoredMove(Move move, int evaluation, int depth, GameState gameState)
    {
        Move = move;
        Evaluation = evaluation;
        Depth = depth;

        GameState = gameState;
    }
}