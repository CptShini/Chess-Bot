using System;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.MaterialEvaluator;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.PositionEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Evaluator
{
    internal const int GameNotOverState = -1;
    internal const int DrawState = 0;
    internal const int CheckmateState = 1;
    
    private static readonly Random Random = new();
    
    private readonly Board _board;

    internal Evaluator(Board board) => _board = board;
    
    internal int EvaluateMove(Move move)
    {
        return EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, _board) + EvaluateEarlyGameRandomness();
        
        int EvaluateEarlyGameRandomness()
        {
            int ply = _board.PlyCount;
            if (ply > RandomExtent) return 0;

            float openingFactor = 1f - (float)ply / RandomExtent;
            int randomness = (int)(openingFactor * RandomStrength);
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
            int drawValue = ComputeDrawValue();
            endEvaluation = drawValue;
            return DrawState;
        }

        endEvaluation = 0;
        return GameNotOverState;
        
        int ComputeDrawValue()
        {
            float earlyGameFactor = 1f - EndgameEvaluator.EndgameFactor(_board);
            return (int)(earlyGameFactor * -ContemptValue);
        }
    }
}