using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;

internal static class ValueboardEvaluator
{
    internal static int EvaluatePiecePositioning(this PieceType pieceType, int positionIndex, int enemyPieceCount)
    {
        Valueboard valueboard = pieceType.GetValueboard();
        return valueboard[enemyPieceCount, positionIndex];
    }
    
    internal static int EvaluateValueboardMove(this Move move, bool isWhiteToMove, int enemyPieceCount)
    {
        int start = move.StartSquare.Index.FlipIndex(isWhiteToMove);
        int target = move.TargetSquare.Index.FlipIndex(isWhiteToMove);
        
        Valueboard valueboard = move.MovePieceType.GetValueboard();
        int startValue = valueboard[enemyPieceCount, start];
        int targetValue = valueboard[enemyPieceCount, target];

        return targetValue - startValue;
    }
}