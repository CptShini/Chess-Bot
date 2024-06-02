using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.PositionEvaluator;
using static Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.MaterialEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

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

        float moveEvaluation = EvaluateOnlyMove(move);
        return (moveEvaluation, false);
    }

    internal float EvaluateOnlyMove(Move move) => (EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, _board)) * Perspective;

    internal (float, bool) EvaluateBoard()
    {
        var (stateEvaluation, isGameEnded) = EvaluateBoardState();
        if (isGameEnded) return (stateEvaluation, true);

        float boardMaterial = EvaluateBoardMaterial(_board) + EvaluateBoardPositioning(_board);
        return (boardMaterial, false);
    }

    private (float, bool) EvaluateBoardState()
    {
        bool isCheckmate = _board.IsInCheckmate();
        if (isCheckmate) return (CheckmateValue * Perspective, true);

        bool isDraw = _board.IsDraw();
        return isDraw ? (DrawValue, true) : (0f, false);
    }
}