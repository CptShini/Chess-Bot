using ChessChallenge.API;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Material;

internal static class MaterialEvaluator
{
    private static readonly float[] _pieceValues = { 0f, 1f, 3f, 3f, 5f, 9f, 100f };

    internal static float EvaluateMoveMaterial(Move move)
    {
        float materialScore = 0f;

        if (move.IsCapture) materialScore += GetPieceValue(move.CapturePieceType);
        if (move.IsPromotion) materialScore += GetPieceValue(move.PromotionPieceType) - 1f;

        return materialScore;
    }

    internal static float EvaluatePieceListsMaterial(PieceList[] allPieceLists)
    {
        float evaluation = 0f;

        foreach (PieceList pieceList in allPieceLists)
        {
            float value = GetPieceValue(pieceList.TypeOfPieceInList);
            int count = pieceList.Count;
            float worth = value * count;

            bool whitePieceList = pieceList.IsWhitePieceList;
            evaluation += whitePieceList ? worth : -worth;
        }

        return evaluation;
    }

    internal static float GetPieceValue(PieceType pieceType) => _pieceValues[(int)pieceType];
}