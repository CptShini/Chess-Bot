using static Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Material.MaterialEvaluator;
using static Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Positioning.PositionEvaluator;
using ChessChallenge.API;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal class Evaluator
{
    private const float CheckmateValue = 100f;
    private const float DrawValue = 0f;

    private readonly Board _board;
    private int Perspective => _board.IsWhiteToMove ? -1 : 1;

    internal Evaluator(Board board) => _board = board;

    internal (float, bool) EvaluateMove(Move move)
    {
        var (stateEvaluation, gameHasEnded) = EvaluateBoardState();
        if (gameHasEnded) return (stateEvaluation, true);

        float moveEvaluation = EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, !_board.IsWhiteToMove);
        return (moveEvaluation * Perspective, false);
    }

    internal (float, bool) EvaluateBoard()
    {
        var (stateEvaluation, isGameEnded) = EvaluateBoardState();
        if (isGameEnded) return (stateEvaluation, true);

        float boardMaterial = EvaluatePieceListsMaterial(_board.GetAllPieceLists());
        return (boardMaterial, false);
    }

    private (float, bool) EvaluateBoardState()
    {
        bool isCheckmate = _board.IsInCheckmate();
        if (isCheckmate) return (CheckmateValue * Perspective, true);

        bool isDraw = _board.IsDraw();
        if (isDraw) return (DrawValue, true);

        return (0f, false);
    }
}