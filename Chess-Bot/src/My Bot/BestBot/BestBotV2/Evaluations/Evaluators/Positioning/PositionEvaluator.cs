using ChessChallenge.API;
using System.Collections.Generic;
using static Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Positioning.Valueboard;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Positioning;

internal static class PositionEvaluator
{
    private static readonly Dictionary<PieceType, float[]> _pieceTypeToValueboard = new() {
        { PieceType.Pawn, PawnValueboard },
        { PieceType.Knight, KnightValueboard }
    };

    private static readonly float _castle = 0.5f;

    internal static float EvaluateMovePositioning(Move move, bool whiteMadeMove)
    {
        float positioning = move.EvaluateMoveValueboard(whiteMadeMove);

        if (move.IsCastles) positioning += _castle;

        return positioning;
    }

    private static float EvaluateMoveValueboard(this Move move, bool whiteMadeMove)
    {
        if (_pieceTypeToValueboard.TryGetValue(move.MovePieceType, out var valueBoard))
        {
            int source = whiteMadeMove ? move.StartSquare.Index : 63 - move.StartSquare.Index;
            int target = whiteMadeMove ? move.TargetSquare.Index : 63 - move.TargetSquare.Index;

            return valueBoard[target] - valueBoard[source];
        }

        return 0f;
    }
}