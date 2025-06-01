using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class PositionEvaluator
{
    internal static int EvaluatePositioning(this Move move, bool isWhiteToMove, int enemyPiecesLeft)
    {
        int positioning = EvaluateValueboardMove();

        if (move.IsCastles) positioning += Castle;

        return positioning;
        
        int EvaluateValueboardMove()
        {
            PieceType pieceType = move.MovePieceType;
            
            int start = move.StartSquare.Index.FlipIndex(isWhiteToMove);
            int target = move.TargetSquare.Index.FlipIndex(isWhiteToMove);
            
            int startPositionValue = pieceType.EvaluatePiecePositioning(start, enemyPiecesLeft);
            int targetPositionValue = pieceType.EvaluatePiecePositioning(target, enemyPiecesLeft);

            return targetPositionValue - startPositionValue;
        }
    }

    internal static int EvaluatePositioning(this Board board, int enemyPiecesLeft)
    {
        int evaluation = 0;
        for (int i = 0; i < 64; i++)
        {
            Square square = new(i);
            Piece piece = board.GetPiece(square);
            if (piece.PieceType == PieceType.None) continue;
            
            bool whitePiece = piece.IsWhite;
            int perspectiveIndex = square.Index.FlipIndex(!whitePiece);
            
            int worth = piece.PieceType.EvaluatePiecePositioning(perspectiveIndex, enemyPiecesLeft);
            evaluation += worth.Perspective(whitePiece);
        }

        return evaluation;
    }

    private static int EvaluatePiecePositioning(this PieceType pieceType, int positionIndex, int enemyPiecesLeft) =>
        PieceValueboards[pieceType].GetValueAt(enemyPiecesLeft, positionIndex);
}