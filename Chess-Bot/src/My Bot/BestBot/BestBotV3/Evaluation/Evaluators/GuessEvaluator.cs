using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators.MaterialEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators;

internal static class GuessEvaluator
{
    internal static IOrderedEnumerable<Move> GuessOrder(this IEnumerable<Move> moves) => moves.OrderByDescending(move => move.GuessEvaluate());

    private static int GuessEvaluate(this Move move)
    {
        int evaluationGuess = 0;
        
        int movingPieceValue = GetPieceValue(move.MovePieceType);
        if (move.IsCapture) evaluationGuess += GetPieceValue(move.CapturePieceType) * 10 - movingPieceValue;
        if (move.IsPromotion) evaluationGuess += GetPieceValue(move.PromotionPieceType) - 100;

        return evaluationGuess;
    }
}