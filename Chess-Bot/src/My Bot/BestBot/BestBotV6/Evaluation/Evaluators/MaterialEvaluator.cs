using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class MaterialEvaluator
{
    internal static int EvaluateMaterial(this Move move)
    {
        int material = 0;

        if (move.IsCapture)
            material += move.CapturePieceType.GetPieceValue();
        
        if (move.IsPromotion)
            material += move.PromotionPieceType.GetPieceValue() - PawnValue;

        return material;
    }

    internal static int EvaluateMaterial(this Board board)
    {
        int evaluation = 0;

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            int pieceValue = pieceList.TypeOfPieceInList.GetPieceValue();
            int count = pieceList.Count;
            int material = pieceValue * count;

            bool whitePieceList = pieceList.IsWhitePieceList;
            int value = material.Perspective(whitePieceList);
            evaluation += value;
        }
        
        return evaluation;
    }
}