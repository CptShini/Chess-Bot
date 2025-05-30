using System.Collections.Generic;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

public static class PieceTypeExtensions
{
    private static readonly Dictionary<PieceType, int> _pieceValues = new()
    {
        { PieceType.Pawn, PawnValue },
        { PieceType.Knight, KnightValue },
        { PieceType.Bishop, BishopValue },
        { PieceType.Rook, RookValue },
        { PieceType.Queen, QueenValue },
        { PieceType.King, KingValue }
    };
    
    internal static int GetPieceValue(this PieceType pieceType) => _pieceValues[pieceType];
}