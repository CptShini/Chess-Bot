using static Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators.Material.MaterialEvaluator;
using ChessChallenge.API;
using System.Collections.Generic;
using System.Linq;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal static class GuessEvaluator
{
    internal static IOrderedEnumerable<Move> GuessOrderMoves(this IEnumerable<Move> moves, Board board) => moves.OrderByDescending(move => GuessEvaluateMove(board, move));

    private static float GuessEvaluateMove(Board board, Move move)
    {
        float evaluationGuess = 0;

        if (move.IsCapture) evaluationGuess += GetPieceValue(move.CapturePieceType);

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