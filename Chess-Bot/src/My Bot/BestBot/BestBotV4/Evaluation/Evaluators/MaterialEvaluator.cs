using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators;

internal static class MaterialEvaluator
{
    private static readonly int[] PieceValues = { 0, 100, 320, 330, 500, 900, 10000 };

    internal static int EvaluateMoveMaterial(Move move)
    {
        int materialScore = 0;

        if (move.IsCapture) materialScore += GetPieceValue(move.CapturePieceType);
        if (move.IsPromotion) materialScore += GetPieceValue(move.PromotionPieceType) - 100;

        return materialScore;
    }

    internal static int EvaluateBoardMaterial(Board board)
    {
        int evaluation = 0;

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            int value = GetPieceValue(pieceList.TypeOfPieceInList);
            int count = pieceList.Count;
            int worth = value * count;

            bool whitePieceList = pieceList.IsWhitePieceList;
            evaluation += whitePieceList ? worth : -worth;
        }
        
        return evaluation;
    }

    internal static int GetPieceValue(PieceType pieceType) => PieceValues[(int)pieceType];
}