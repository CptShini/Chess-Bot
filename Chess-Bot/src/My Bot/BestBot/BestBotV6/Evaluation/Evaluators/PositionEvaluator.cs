using Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class PositionEvaluator
{
    internal static int EvaluatePositioning(this Move move, bool isWhiteToMove, int enemyPiecesLeft)
    {
        int positioning = move.EvaluateValueboardMove(isWhiteToMove, enemyPiecesLeft);

        if (move.IsCastles) positioning += CastleValue;

        return positioning;
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
            int positioning = piece.PieceType.EvaluatePiecePositioning(perspectiveIndex, enemyPiecesLeft);
            
            int value = positioning.Perspective(whitePiece);
            evaluation += value;
        }

        return evaluation;
    }
}