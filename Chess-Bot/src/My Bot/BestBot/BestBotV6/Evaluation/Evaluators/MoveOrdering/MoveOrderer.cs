using System;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.MoveOrdering;

internal static class MoveOrderer
{
    private static readonly int[] _moveScores = new int[128];
    
    internal static void OrderMoves(this Board board, Span<Move> moves, Move pvMove, int enemyPieceCount)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            Move move = moves[i];
            if (move == pvMove)
            {
                _moveScores[i] = pvMoveBias;
                continue;
            }
            
            _moveScores[i] = ScoreMove(move, board, enemyPieceCount);
        }
        
        Sorting.Quicksort(moves, _moveScores);
    }

    private static int ScoreMove(Move move, Board board, int enemyPieceCount)
    {
        int score = 0;
        if (move.IsCapture) score += ScoreCapture(move, board);

        if (move is { MovePieceType: PieceType.Pawn, IsPromotion: true }
            and { PromotionPieceType: PieceType.Queen, IsCapture: false })
            score += promoteBias;
        else
            score += move.EvaluatePositioning(board.IsWhiteToMove, enemyPieceCount);

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
}