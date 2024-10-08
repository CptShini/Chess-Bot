﻿using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal static class MaterialEvaluator
{
    private static readonly float[] PieceValues = { 0f, 1f, 3.2f, 3.3f, 5f, 9f, 100f };

    internal static float EvaluateMoveMaterial(Move move)
    {
        float materialScore = 0f;

        if (move.IsCapture) materialScore += GetPieceValue(move.CapturePieceType);
        if (move.IsPromotion) materialScore += GetPieceValue(move.PromotionPieceType) - 1f;

        return materialScore;
    }

    internal static float EvaluateBoardMaterial(Board board)
    {
        float evaluation = 0f;

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            float value = GetPieceValue(pieceList.TypeOfPieceInList);
            int count = pieceList.Count;
            float worth = value * count;

            bool whitePieceList = pieceList.IsWhitePieceList;
            evaluation += whitePieceList ? worth : -worth;
        }

        return evaluation;
    }

    internal static float GetPieceValue(PieceType pieceType) => PieceValues[(int)pieceType];
}