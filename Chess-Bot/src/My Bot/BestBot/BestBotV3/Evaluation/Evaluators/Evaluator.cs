using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators.MaterialEvaluator;
using static Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators.PositionEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV3.Evaluation.Evaluators;

internal static class Evaluator
{
    private const int CheckmateValue = -10000;
    private const int DrawValue = 0;

    internal static int EvaluateMove(Move move, Board board) => EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, board);
    
    internal static int EvaluateBoard(Board board) => EvaluateBoardMaterial(board) + EvaluateBoardPositioning(board);

    internal static int EvaluateBoardState(Board board, out bool gameHasEnded)
    {
        gameHasEnded = true;
        
        bool isCheckmate = board.IsInCheckmate();
        if (isCheckmate) return CheckmateValue;

        bool isDraw = board.IsDraw();
        if (isDraw) return DrawValue;

        gameHasEnded = false;
        return 0;
    }
}