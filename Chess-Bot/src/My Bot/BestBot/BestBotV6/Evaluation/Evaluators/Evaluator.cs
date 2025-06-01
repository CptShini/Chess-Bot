using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Evaluator
{
    private readonly Board _board;

    internal Evaluator(Board board) => _board = board;
    
    internal int EvaluateMove(Move move, int enemyPiecesLeft)
    {
        int moveValue =
            move.EvaluateMaterial() +
            move.EvaluatePositioning(_board.IsWhiteToMove, enemyPiecesLeft);

        int ply = _board.PlyCount;
        if (ply <= Random_PlyExtent)
            moveValue += EvaluateEarlyGameRandomness();
        
        return moveValue;
        
        int EvaluateEarlyGameRandomness()
        {
            int randomness = (int)(OpeningFactor(ply) * Random_Strength);
            return Random.Next(-randomness, randomness + 1);
        }
    }

    internal int EvaluateBoard(int enemyPiecesLeft) =>
        _board.EvaluateMaterial() + _board.EvaluatePositioning(enemyPiecesLeft);
}