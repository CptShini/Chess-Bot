using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.MyBots.MyBotV1;

public class MyBotV1 : IChessBot
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
        IEnumerable<ScoredMove> scoredMoves = EvaluateBoardMoves(board, 0);

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

    IEnumerable<ScoredMove> EvaluateBoardMoves(Board board, int depth)
    {
        List<ScoredMove> scoredMoves = new();

        IEnumerable<Move> legalMoves = board.GetLegalMoves();
        foreach (Move legalMove in legalMoves)
        {
            board.MakeMove(legalMove);
            float moveScore = EvaluateBoard(board, depth);
            board.UndoMove(legalMove);

            ScoredMove scoredMove = new ScoredMove(legalMove, moveScore);
            scoredMoves.Add(scoredMove);
        }

        return scoredMoves;
    }

    float EvaluateBoard(Board board, int depth)
    {
        if (board.IsInCheckmate()) return board.IsWhiteToMove ? -_checkmateValue : _checkmateValue;
        if (board.IsDraw()) return _drawValue;

        if (depth == _maxDepth) return EvaluateBoard(board);

        IEnumerable<ScoredMove> scoredMoves = EvaluateBoardMoves(board, ++depth);
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