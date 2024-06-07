using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators.MaterialEvaluator;
using static Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators.PositionEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation.Evaluators;

internal static class Evaluator
{
    internal static int EvaluateMove(Move move, Board board) => EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, board);
    
    internal static int EvaluateBoard(Board board) => EvaluateBoardMaterial(board) + EvaluateBoardPositioning(board);

    internal static int EvaluateBoardState(Board board)
    {
        bool isCheckmate = board.IsInCheckmate();
        if (isCheckmate) return 2;

        bool isDraw = board.IsDraw();
        return isDraw ? 1 : 0;
    }
}