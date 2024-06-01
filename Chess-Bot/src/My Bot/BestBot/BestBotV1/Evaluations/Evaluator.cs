using static Chess_Challenge.src.My_Bot.BestBot.BestBotV1.Evaluations.Material.MaterialEvaluator;
using static Chess_Challenge.src.My_Bot.BestBot.BestBotV1.Evaluations.Positioning.PositionEvaluator;
using ChessChallenge.API;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV1.Evaluations;

internal static class Evaluator
{
    private const float CheckmateValue = 100f;
    private const float DrawValue = 0f;

    internal static (float, bool) EvaluateMove(Board board, Move move)
    {
        bool whiteMadeMove = !board.IsWhiteToMove;

        var (stateEvaluation, gameHasEnded) = EvaluateBoardState(board);
        if (gameHasEnded) return (stateEvaluation, true);

        float moveEvaluation = EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, whiteMadeMove);
        return (moveEvaluation.FlipSign(whiteMadeMove), false);
    }

    internal static (float, bool) EvaluateBoard(Board board)
    {
        var (stateEvaluation, isGameEnded) = EvaluateBoardState(board);
        if (isGameEnded) return (stateEvaluation, true);

        float boardMaterial = EvaluatePieceListsMaterial(board.GetAllPieceLists());
        return (boardMaterial, false);
    }

    private static (float, bool) EvaluateBoardState(Board board)
    {
        return board.IsInCheckmate()
            ? (CheckmateValue.FlipSign(!board.IsWhiteToMove), true)
            : board.IsDraw() ? (DrawValue, true) : (0f, false);
    }

    private static float FlipSign(this float value, bool whiteMadeMove) => whiteMadeMove ? value : -value;
}