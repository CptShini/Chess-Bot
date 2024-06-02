using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.MaterialEvaluator;
using static Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.PositionEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal static class GuessEvaluator
{
    internal static IOrderedEnumerable<Move> GuessOrder(this IEnumerable<Move> moves, Board board) => moves.OrderByDescending(move => move.GuessEvaluate(board));

    private static float GuessEvaluate(this Move move, Board board)
    {
        float evaluationGuess = EvaluateMoveMaterial(move) + EvaluateMovePositioning(move, board);

        float movingPieceValue = GetPieceValue(move.MovePieceType);
        bool targetSquareAttacked = board.SquareIsAttackedByOpponent(move.TargetSquare);
        if (targetSquareAttacked) evaluationGuess -= movingPieceValue;
        else
        {
            bool startSquareAttacked = board.SquareIsAttackedByOpponent(move.StartSquare);
            if (startSquareAttacked) evaluationGuess += movingPieceValue;
        }

        return evaluationGuess;
    }
}