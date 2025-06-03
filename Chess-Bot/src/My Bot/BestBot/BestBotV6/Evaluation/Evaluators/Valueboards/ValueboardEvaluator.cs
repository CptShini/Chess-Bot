using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Evaluators.Valueboards;

internal static class ValueboardEvaluator
{
    internal static int EvaluatePiecePositioning(this PieceType pieceType, int positionIndex, int enemyPiecesLeft)
    {
        Valueboard valueboard = pieceType.GetValueboard();
        return valueboard[enemyPiecesLeft, positionIndex];
    }
    
    internal static int EvaluateValueboardMove(this Move move, bool isWhiteToMove, int enemyPiecesLeft)
    {
        int start = move.StartSquare.Index.FlipIndex(isWhiteToMove);
        int target = move.TargetSquare.Index.FlipIndex(isWhiteToMove);
        
        Valueboard valueboard = move.MovePieceType.GetValueboard();
        int startValue = valueboard[enemyPiecesLeft, start];
        int targetValue = valueboard[enemyPiecesLeft, target];

        return targetValue - startValue;
    }
}