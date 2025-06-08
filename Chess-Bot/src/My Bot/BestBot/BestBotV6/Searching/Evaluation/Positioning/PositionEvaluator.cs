using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning.Valueboards;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning;

internal static class PositionEvaluator
{
    internal static int EvaluatePositioning(this Move move, bool isWhiteToMove, int enemyPieceCount)
    {
        int positioning = move.EvaluateValueboardMove(isWhiteToMove, enemyPieceCount);

        if (move.IsCastles) positioning += CastleValue;

        return positioning;
    }

    internal static int EvaluatePositioning(this Board board, int enemyPieceCount)
    {
        int evaluation = 0;
        
        for (int i = 0; i < 64; i++)
        {
            Square square = new(i);
            Piece piece = board.GetPiece(square);
            if (piece.PieceType == PieceType.None) continue;
            
            bool whitePiece = piece.IsWhite;
            int perspectiveIndex = square.Index.FlipIndex(!whitePiece);
            int positioning = piece.PieceType.EvaluateValueboardPosition(perspectiveIndex, enemyPieceCount);
            
            int value = positioning.Perspective(whitePiece);
            evaluation += value;
        }

        return evaluation;
    }
}