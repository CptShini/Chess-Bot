using ChessChallenge.API;

namespace Chess_Challenge.src.My_Bot.BestBot.BestBotV2.Evaluations.Evaluators;

internal static class EndgameEvaluator
{
    internal static float EndgameFactor(Board board) => (16 - CountEnemyPieces(board)) / 15f;

    private static int CountEnemyPieces(Board board)
    {
        int count = 0;

        bool whiteToMove = board.IsWhiteToMove;
        foreach (PieceList pieceList in board.GetAllPieceLists())
        {
            bool whitePieceList = pieceList.IsWhitePieceList;

            bool enemyPieceList = whiteToMove != whitePieceList;
            if (enemyPieceList) count += pieceList.Count;
        }

        return count;
    }
}