using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards.Valueboards;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class PositionEvaluator
{
    private const int Castle = 50;

    internal static int EvaluateMovePositioning(Move move, Board board)
    {
        int positioning = move.EvaluateMoveValueboard(board);

        if (move.IsCastles) positioning += Castle;

        return positioning;
    }

    private static int EvaluateMoveValueboard(this Move move, Board board)
    {
        PieceType pieceType = move.MovePieceType;

        bool isWhiteToMove = board.IsWhiteToMove;
        int start = move.StartSquare.PerspectiveIndex(!isWhiteToMove);
        int target = move.TargetSquare.PerspectiveIndex(!isWhiteToMove);
        
        int startPositionValue = EvaluatePiecePositioning(board, pieceType, start);
        int targetPositionValue = EvaluatePiecePositioning(board, pieceType, target);

        return targetPositionValue - startPositionValue;
    }

    internal static int EvaluateBoardPositioning(Board board)
    {
        int evaluation = 0;

        for (int i = 0; i < 64; i++)
        {
            Square iSquare = new(i);
            Piece iPiece = board.GetPiece(iSquare);

            bool whitePiece = iPiece.IsWhite;
            int perspectiveIndex = iSquare.PerspectiveIndex(whitePiece);

            int worth = EvaluatePiecePositioning(board, iPiece.PieceType, perspectiveIndex);

            evaluation += whitePiece ? worth : -worth;
        }

        return evaluation;
    }

    private static int EvaluatePiecePositioning(Board board, PieceType pieceType, int positionIndex)
    {
        bool pieceTypeHasValueboard = PieceValueboards.TryGetValue(pieceType, out var valueBoard);
        if (!pieceTypeHasValueboard) return 0;

        float endgameFactor = EndgameEvaluator.EndgameFactor(board);
        return valueBoard.GetValueAt(endgameFactor, positionIndex);
    }

    private static int PerspectiveIndex(this Square square, bool whitePerspective) => whitePerspective ? square.Index : 63 - square.Index;
}