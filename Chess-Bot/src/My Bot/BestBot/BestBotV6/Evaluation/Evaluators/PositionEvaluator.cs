using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class PositionEvaluator
{
    internal static int EvaluatePositioning(this Move move, bool isWhiteToMove, float endgameFactor)
    {
        int positioning = EvaluateValueboardMove();

        if (move.IsCastles) positioning += Castle;

        return positioning;
        
        int EvaluateValueboardMove()
        {
            PieceType pieceType = move.MovePieceType;
        
            int start = move.StartSquare.PerspectiveIndex(!isWhiteToMove);
            int target = move.TargetSquare.PerspectiveIndex(!isWhiteToMove);
        
            int startPositionValue = pieceType.EvaluatePiecePositioning(start, endgameFactor);
            int targetPositionValue = pieceType.EvaluatePiecePositioning(target, endgameFactor);

            return targetPositionValue - startPositionValue;
        }
    }

    internal static int EvaluatePositioning(this Board board, float endgameFactor)
    {
        int evaluation = 0;
        for (int i = 0; i < 64; i++)
        {
            Square square = new(i);
            Piece piece = board.GetPiece(square);
            if (piece.PieceType == PieceType.None) continue;

            bool whitePiece = piece.IsWhite;
            int perspectiveIndex = square.PerspectiveIndex(whitePiece);

            int worth = piece.PieceType.EvaluatePiecePositioning(perspectiveIndex, endgameFactor);

            evaluation += whitePiece ? worth : -worth;
        }

        return evaluation;
    }

    private static int EvaluatePiecePositioning(this PieceType pieceType, int positionIndex, float endgameFactor) =>
        PieceValueboards[pieceType].GetValueAt(endgameFactor, positionIndex);

    private static int PerspectiveIndex(this Square square, bool whitePerspective) => whitePerspective ? square.Index : 63 - square.Index;
}