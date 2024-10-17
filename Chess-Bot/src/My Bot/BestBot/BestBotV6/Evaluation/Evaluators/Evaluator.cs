using System;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.MaterialEvaluator;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.PositionEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Evaluator
{
    private const int CheckmateValue = -10000;
    private const int ContemptValue = -50;

    internal const int DrawState = 0;
    internal const int CheckmateState = 1;
    
    // Opening randomness
    private const int extent = 5;
    private const int strength = 60;
    private static readonly Random Random = new();
    
    private readonly Board _board;

    internal Evaluator(Board board) => _board = board;
    
    internal int EvaluateMove(Move move)
    {
        return EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, _board);// + EvaluateEarlyGameRandomness();
        
        int EvaluateEarlyGameRandomness()
        {
            int ply = _board.PlyCount;
            if (ply > extent) return 0;

            float openingFactor = 1f - (float)ply / extent;
            int randomness = (int)(openingFactor * strength);
            return Random.Next(-randomness, randomness + 1);
        }
    }

    internal int EvaluateBoard() => EvaluateBoardMaterial(_board) + EvaluateBoardPositioning(_board);

    internal int EvaluateBoardState(out int endEvaluation)
    {
        bool isCheckmate = _board.IsInCheckmate();
        if (isCheckmate)
        {
            endEvaluation = CheckmateValue;
            return CheckmateState;
        }

        bool isDraw = _board.IsDraw();
        if (isDraw)
        {
            float earlyGameFactor = 1f - EndgameEvaluator.EndgameFactor(_board);
            int drawValue = (int)(earlyGameFactor * -ContemptValue);
            endEvaluation = drawValue;
            return DrawState;
        }

        endEvaluation = 0;
        return -1;
    }
}