using Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Valueboards;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Valueboards.Valueboards;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal static class PositionEvaluator
{
    private const float Castle = 0.5f;

    internal static float EvaluateMovePositioning(Move move, Board board)
    {
        float positioning = move.EvaluateMoveValueboard(board);

        if (move.IsCastles) positioning += Castle;

        return positioning;
    }

    private static float EvaluateMoveValueboard(this Move move, Board board)
    {
        PieceType pieceType = move.MovePieceType;

        int start = move.StartSquare.PerspectiveIndex(!board.IsWhiteToMove);
        int target = move.TargetSquare.PerspectiveIndex(!board.IsWhiteToMove);
        
        float startPositionValue = EvaluatePiecePositioning(board, pieceType, start);
        float targetPositionValue = EvaluatePiecePositioning(board, pieceType, target);

        return targetPositionValue - startPositionValue;
    }

    internal static float EvaluateBoardPositioning(Board board)
    {
        float evaluation = 0f;

        for (int i = 0; i < 64; i++)
        {
            Square iSquare = new(i);
            Piece iPiece = board.GetPiece(iSquare);

            bool whitePiece = iPiece.IsWhite;
            int perspectiveIndex = iSquare.PerspectiveIndex(whitePiece);

            float worth = EvaluatePiecePositioning(board, iPiece.PieceType, perspectiveIndex);

            evaluation += whitePiece ? worth : -worth;
        }

        return evaluation;
    }

    private static float EvaluatePiecePositioning(Board board, PieceType pieceType, int positionIndex)
    {
        bool pieceTypeHasValueboard = PieceValueboards.TryGetValue(pieceType, out var valueBoard);
        if (!pieceTypeHasValueboard) return 0f;

        float endgameFactor = EndgameEvaluator.EndgameFactor(board);
        return valueBoard.GetValueAt(endgameFactor, positionIndex);
    }

    private static int PerspectiveIndex(this Square square, bool whitePerspective) => whitePerspective ? square.Index : 63 - square.Index;
}