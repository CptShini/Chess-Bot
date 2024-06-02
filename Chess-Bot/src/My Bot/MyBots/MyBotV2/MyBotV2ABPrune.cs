using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.MyBots.MyBotV2;

public class MyBotV2ABPrune : IChessBot
{
    private static readonly int _maxDepth = 3;

    private static readonly float[] _pieceValues = { 0f, 1f, 3f, 3f, 5f, 9f, 100f };
    private const float _checkmateValue = float.MaxValue;
    private const float _drawValue = 0f;

    private static readonly Random _random = new();

    public Move Think(Board board, Timer timer)
    {
        Move bestMove = GetBestMove(board);

        return bestMove;
    }

    Move GetBestMove(Board board)
    {
        IEnumerable<ScoredMove> scoredMoves = EvaluateBoardMoves(board, 0, float.MinValue, float.MaxValue);

        return GetBestScoredMove(scoredMoves, board);
    }

    Move GetBestScoredMove(IEnumerable<ScoredMove> scoredMoves, Board board)
    {
        float maxEval = board.IsWhiteToMove ? scoredMoves.Max(sm => sm.Score) : scoredMoves.Min(sm => sm.Score);
        IEnumerable<ScoredMove> bestMoves = scoredMoves.Where(sm => sm.Score == maxEval);

        int i = _random.Next(bestMoves.Count());
        ScoredMove bestMove = bestMoves.ElementAt(i);

        return bestMove.Move;
    }

    IEnumerable<ScoredMove> EvaluateBoardMoves(Board board, int depth, float alpha, float beta)
    {
        List<ScoredMove> scoredMoves = new();

        IEnumerable<Move> orderedLegalMoves = board.GetLegalMoves().OrderByDescending(m => MoveScoreGuess(board, m));
        foreach (Move legalMove in orderedLegalMoves)
        {
            board.MakeMove(legalMove);
            float moveScore = EvaluateBoard(board, depth, alpha, beta);
            board.UndoMove(legalMove);

            ScoredMove scoredMove = new ScoredMove(legalMove, moveScore);
            scoredMoves.Add(scoredMove);

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

    float EvaluateBoard(Board board, int depth, float alpha, float beta)
    {
        if (board.IsInCheckmate()) return board.IsWhiteToMove ? -_checkmateValue : _checkmateValue;
        if (board.IsDraw()) return _drawValue;

        if (depth == _maxDepth) return EvaluateBoard(board);

        IEnumerable<ScoredMove> scoredMoves = EvaluateBoardMoves(board, ++depth, alpha, beta);
        float score = board.IsWhiteToMove ? scoredMoves.Max(sm => sm.Score) : scoredMoves.Min(sm => sm.Score);

        return score;
    }

    float EvaluateBoard(Board board)
    {
        float evalSum = 0f;

        evalSum += EvaluateMaterial(board);

        return evalSum;
    }

    float EvaluateMaterial(Board board)
    {
        float white = 0f, black = 0f;

        for (int i = 0; i < 64; i++)
        {
            Square square = new(i);
            Piece piece = board.GetPiece(square);

            float pieceVal = EvaluatePieceMaterial(piece);
            if (piece.IsWhite) white += pieceVal;
            else black += pieceVal;
        }

        float val = white - black;

        return val;
    }

    float EvaluatePieceMaterial(Piece piece) => _pieceValues[(int)piece.PieceType];
}