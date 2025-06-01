using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class MaterialEvaluator
{
    internal static int EvaluateMaterial(this Move move)
    {
        int materialScore = 0;

        if (move.IsCapture) materialScore += move.CapturePieceType.GetPieceValue();
        if (move.IsPromotion) materialScore += move.PromotionPieceType.GetPieceValue() - PawnValue;

        return materialScore;
    }

    internal static int EvaluateMaterial(this Board board)
    {
        int evaluation = 0;

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            int value = pieceList.TypeOfPieceInList.GetPieceValue();
            int count = pieceList.Count;
            int worth = value * count;

            bool whitePieceList = pieceList.IsWhitePieceList;
            evaluation += worth.Perspective(whitePieceList);
        }
        
        return evaluation;
    }
}