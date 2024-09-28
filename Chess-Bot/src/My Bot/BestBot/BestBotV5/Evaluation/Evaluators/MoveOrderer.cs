using System;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation.Evaluators.MaterialEvaluator;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation.Evaluators;

internal static class MoveOrderer
{
    private static readonly int[] _moveScores = new int[128];
    
    private const int million = 1_000_000;
    private const int pvMoveBias = 100 * million;
    private const int winningCaptureBias = 8 * million;
    private const int promoteBias = 6 * million;
    private const int losingCaptureBias = 2 * million;
    
    internal static void OrderMoves(Board board, Span<Move> moves, Move pvMove)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            Move move = moves[i];
            if (move == pvMove)
            {
                _moveScores[i] = pvMoveBias;
                continue;
            }
            
            _moveScores[i] = ScoreMove(move, board);
        }
        
        Quicksort(moves, _moveScores, 0, moves.Length - 1);
    }

    private static int ScoreMove(Move move, Board board)
    {
        int score = 0;
        if (move.IsCapture) score += ScoreCapture(move, board);

        if (move is { MovePieceType: PieceType.Pawn, IsPromotion: true } and { PromotionPieceType: PieceType.Queen, IsCapture: false }) score += promoteBias;
        else score += PositionEvaluator.EvaluateMovePositioning(move, board);

        return score;
    }

    private static int ScoreCapture(Move move, Board board)
    {
        int score = 0;
        
        int captureMaterialDelta = GetPieceValue(move.CapturePieceType) - GetPieceValue(move.MovePieceType);
        score += captureMaterialDelta;
                
        bool opponentCanRecapture = board.SquareIsAttackedByOpponent(move.TargetSquare);
        if (opponentCanRecapture) score += captureMaterialDelta >= 0 ? winningCaptureBias : losingCaptureBias;
        else score += winningCaptureBias;

        return score;
    }
    
    private static void Quicksort(Span<Move> values, int[] scores, int low, int high)
    {
        if (low >= high) return;
        
        int pivotIndex = Partition(values, scores, low, high);
        Quicksort(values, scores, low, pivotIndex - 1);
        Quicksort(values, scores, pivotIndex + 1, high);
    }

    private static int Partition(Span<Move> values, int[] scores, int low, int high)
    {
        int pivotScore = scores[high];
        int i = low - 1;

        for (int j = low; j <= high - 1; j++)
        {
            if (scores[j] <= pivotScore) continue;
            
            i++;
            (values[i], values[j]) = (values[j], values[i]);
            (scores[i], scores[j]) = (scores[j], scores[i]);
        }
        (values[i + 1], values[high]) = (values[high], values[i + 1]);
        (scores[i + 1], scores[high]) = (scores[high], scores[i + 1]);

        return i + 1;
    }
}