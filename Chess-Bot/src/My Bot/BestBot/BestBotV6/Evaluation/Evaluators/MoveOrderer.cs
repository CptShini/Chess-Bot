using System;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators;

internal static class MoveOrderer
{
    private static readonly int[] _moveScores = new int[128];
    
    internal static void OrderMoves(this Board board, Span<Move> moves, Move pvMove, float endgameFactor)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            Move move = moves[i];
            if (move == pvMove)
            {
                _moveScores[i] = pvMoveBias;
                continue;
            }
            
            _moveScores[i] = ScoreMove(move, board, endgameFactor);
        }
        
        Quicksort(moves, _moveScores, 0, moves.Length - 1);
    }

    private static int ScoreMove(Move move, Board board, float endgameFactor)
    {
        int score = 0;
        if (move.IsCapture) score += ScoreCapture(move, board);

        if (move is { MovePieceType: PieceType.Pawn, IsPromotion: true }
            and { PromotionPieceType: PieceType.Queen, IsCapture: false })
            score += promoteBias;
        else
            score += move.EvaluatePositioning(board.IsWhiteToMove, endgameFactor);

        return score;
    }

    private static int ScoreCapture(Move move, Board board)
    {
        int score = 0;
        
        int captureMaterialDelta = move.CapturePieceType.GetPieceValue() - move.MovePieceType.GetPieceValue();
        score += captureMaterialDelta;
                
        bool opponentCanRecapture = board.SquareIsAttackedByOpponent(move.TargetSquare);
        if (opponentCanRecapture) score += captureMaterialDelta >= 0 ? winningCaptureBias : losingCaptureBias;
        else score += winningCaptureBias;

        return score;
    }
    
    private static void Quicksort(Span<Move> values, int[] scores, int low, int high)
    {
        while (low < high)
        {
            if (high - low <= InsertionSortThreshold)
            {
                InsertionSort(values, scores, low, high);
                break;
            }

            int pivotIndex = Partition(values, scores, low, high);
            if (pivotIndex - low < high - pivotIndex)
            {
                Quicksort(values, scores, low, pivotIndex - 1);
                low = pivotIndex + 1;
            }
            else
            {
                Quicksort(values, scores, pivotIndex + 1, high);
                high = pivotIndex - 1;
            }
        }
    }

    private static int Partition(Span<Move> values, int[] scores, int low, int high)
    {
        int pivotScore = scores[high];
        int i = low;

        for (int j = low; j < high; j++)
        {
            if (scores[j] <= pivotScore) continue;
            
            (values[i], values[j]) = (values[j], values[i]);
            (scores[i], scores[j]) = (scores[j], scores[i]);
            i++;
        }
        
        (values[i], values[high]) = (values[high], values[i]);
        (scores[i], scores[high]) = (scores[high], scores[i]);

        return i;
    }
    
    private static void InsertionSort(Span<Move> values, int[] scores, int low, int high)
    {
        for (int i = low + 1; i <= high; i++)
        {
            var key = values[i];
            int keyScore = scores[i];
            int j = i - 1;
            while (j >= low && scores[j] < keyScore)
            {
                values[j + 1] = values[j];
                scores[j + 1] = scores[j];
                j--;
            }
            values[j + 1] = key;
            scores[j + 1] = keyScore;
        }
    }
}