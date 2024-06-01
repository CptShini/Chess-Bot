using ChessChallenge.API;
using System.Collections.Generic;
using System.Linq;

namespace MyBots.MyBotV3;

public class MyBotV3ABPruneOp : IChessBot
{
    public static readonly int FutureThinkTurns = 1;
    private static readonly int _maxDepth = FutureThinkTurns * 2 + 1;

    private static readonly float[] _pieceValues = { 0f, 1f, 3f, 3f, 5f, 9f, 100f };
    private const float _checkmateValue = float.MaxValue;
    private const float _drawValue = 0f;

    public Move Think(Board board, Timer timer) => GetBestMove(board);

    Move GetBestMove(Board board)
    {
        ScoredMoveList scoredMoves = EvaluateBoardMoves(board);

        return scoredMoves.GetMove(board.IsWhiteToMove);
    }

    ScoredMoveList EvaluateBoardMoves(Board board, int depth = 0, float alpha = float.MinValue, float beta = float.MaxValue)
    {
        ScoredMoveList scoredMoves = new();

        IEnumerable<Move> orderedLegalMoves = depth < _maxDepth ? board.GetLegalMoves().OrderByDescending(m => MoveScoreGuess(board, m)) : board.GetLegalMoves();
        foreach (Move legalMove in orderedLegalMoves)
        {
            board.MakeMove(legalMove);
            float moveScore = EvaluateBoardRecursive(board, depth, alpha, beta);
            board.UndoMove(legalMove);

            scoredMoves.Add(legalMove, moveScore);

            if (board.IsWhiteToMove && moveScore > alpha) alpha = moveScore;
            if (!board.IsWhiteToMove && moveScore < beta) beta = moveScore;

            if (!board.IsWhiteToMove && moveScore < alpha || board.IsWhiteToMove && moveScore > beta) break;
        }

        return scoredMoves;
    }

    float MoveScoreGuess(Board board, Move move)
    {
        float scoreGuess = 0f;
        float movingPieceValue = _pieceValues[(int)move.MovePieceType];
        float targetPieceValue = _pieceValues[(int)move.CapturePieceType];

        scoreGuess += targetPieceValue;
        if (board.SquareIsAttackedByOpponent(move.TargetSquare)) scoreGuess -= movingPieceValue;
        else if (board.SquareIsAttackedByOpponent(move.StartSquare)) scoreGuess += movingPieceValue;

        return scoreGuess;
    }

    float EvaluateBoardRecursive(Board board, int depth, float alpha, float beta)
    {
        if (depth == _maxDepth) return EvaluateMaterial(board);

        if (board.IsInCheckmate()) return board.IsWhiteToMove ? -_checkmateValue : _checkmateValue;
        if (board.IsDraw()) return _drawValue;

        ScoredMoveList scoredMoves = EvaluateBoardMoves(board, depth + 1, alpha, beta);
        return scoredMoves.GetEval(board.IsWhiteToMove);
    }

    float EvaluateMaterial(Board board)
    {
        float material = 0f;

        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            float piecesVal = _pieceValues[(int)pieceList.TypeOfPieceInList] * pieceList.Count;

            material += pieceList.IsWhitePieceList ? piecesVal : -piecesVal;
        }

        return material;
    }
}