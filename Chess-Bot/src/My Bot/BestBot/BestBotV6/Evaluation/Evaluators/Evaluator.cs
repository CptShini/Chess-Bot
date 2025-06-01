using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Evaluator
{
    private readonly Board _board;

    internal Evaluator(Board board) => _board = board;
    
    internal int EvaluateMove(Move move, float endgameFactor)
    {
        return
            move.EvaluateMaterial() +
            move.EvaluatePositioning(_board.IsWhiteToMove, endgameFactor) +
            EvaluateEarlyGameRandomness();
        
        int EvaluateEarlyGameRandomness()
        {
            int ply = _board.PlyCount;
            if (ply > Random_PlyExtent) return 0;
            
            int randomness = (int)(OpeningFactor(ply) * Random_Strength);
            return Random.Next(-randomness, randomness + 1);
        }
    }

    internal int EvaluateBoard(float endgameFactor) =>
        _board.EvaluateMaterial() + _board.EvaluatePositioning(endgameFactor);
}