using ChessChallenge.API;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Positioning;

internal static class PositionEvaluator
{
    private static readonly float _castle = 0.5f;

    internal static float EvaluateMovePositioning(Move move, bool whiteMadeMove)
    {
        float positioning = move.EvaluateMoveValueboard(whiteMadeMove);

        if (move.IsCastles) positioning += _castle;

        return positioning;
    }

    private static float EvaluateMoveValueboard(this Move move, bool whiteMadeMove)
    {
        PieceType pieceType = move.MovePieceType;

        int start = move.StartSquare.PerspectiveIndex(whiteMadeMove);
        int target = move.TargetSquare.PerspectiveIndex(whiteMadeMove);

        float startPositionValue = EvaluatePiecePositioning(pieceType, start);
        float targetPositionValue = EvaluatePiecePositioning(pieceType, target);

        return targetPositionValue - startPositionValue;
    }

    internal static float EvaluateBoardPositioning(Board board)
    {
        float evaluation = 0f;

        for (int i = 0; i < 64; i++)
        {
            Square iSquare = new Square(i);
            Piece iPiece = board.GetPiece(iSquare);

            bool whitePiece = iPiece.IsWhite;
            int perspectiveIndex = iSquare.PerspectiveIndex(whitePiece);

            float worth = EvaluatePiecePositioning(iPiece.PieceType, perspectiveIndex);

            evaluation += whitePiece ? worth : -worth;
        }

        return evaluation;
    }

    private static float EvaluatePiecePositioning(PieceType pieceType, int positionIndex) => Valueboard.PieceValueboards.TryGetValue(pieceType, out var valueBoard) ? valueBoard[positionIndex] / 100f : 0f;

    private static int PerspectiveIndex(this Square square, bool whitePerspective) => whitePerspective ? square.Index : 63 - square.Index;
}