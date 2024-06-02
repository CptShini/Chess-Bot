using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV1;

internal static class Evaluator
{
    private static readonly float[] _pieceValues = { 0f, 1f, 3f, 3f, 5f, 9f, 100f };
    private static readonly float _checkmate = GetPieceValue(PieceType.King);
    private static readonly float _draw = 0f;

    /* Also evaluate:
     * Casteling
     * Promotions
     * Captures
     * Positioning (where on the board)
     * Mobility (how many moves)
     * Attacking / pieces being guarded
     * Trades
     */

    internal static float EvaluateBoard(Board board)
    {
        if (board.IsInCheckmate()) return board.IsWhiteToMove ? -_checkmate : _checkmate;
        if (board.IsDraw()) return _draw;

        float evaluation = 0f;

        evaluation += EvaluateMaterial(board);

        return evaluation;
    }

    #region Material

    private static float EvaluateMaterial(Board board)
    {
        float material = 0f;

        PieceList[] allPieceLists = board.GetAllPieceLists();
        foreach (PieceList pieceList in allPieceLists)
        {
            material += GetPieceListWorth(pieceList);
        }

        return material;
    }

    private static float GetPieceListWorth(PieceList pieceList)
    {
        float value = GetPieceValue(pieceList.TypeOfPieceInList);
        int count = pieceList.Count;
        float worth = value * count;

        bool white = pieceList.IsWhitePieceList;
        return white ? worth : -worth;
    }

    private static float GetPieceValue(PieceType pieceType) => _pieceValues[(int)pieceType];

    #endregion
}