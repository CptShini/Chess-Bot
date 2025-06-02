using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal class Evaluator
{
    private readonly Board _board;

    internal Evaluator(Board board) => _board = board;
    
    internal int EvaluateMove(Move move, int enemyPiecesLeft) =>
        move.EvaluateMaterial() +
        move.EvaluatePositioning(_board.IsWhiteToMove, enemyPiecesLeft);

    internal int EvaluateBoard(int enemyPiecesLeft) =>
        _board.EvaluateMaterial() +
        _board.EvaluatePositioning(enemyPiecesLeft);
}