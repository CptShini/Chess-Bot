using ChessChallenge.API;
using static Chess_Challenge.My_Bot.CleverBot.CleverBotV3.Evaluations.Valueboard;

namespace Chess_Challenge.My_Bot.CleverBot.CleverBotV3.Evaluations;

internal static class Evaluator
{
    private static readonly float[] _pieceValues = { 0f, 1f, 3f, 3f, 5f, 9f, 100f };

    private static readonly float _checkmate = GetPieceValue(PieceType.King);
    private static readonly float _draw = 0f;

    private static readonly float _castle = 0.5f;

    internal static float EvaluateMove(Board board, Move move, bool whiteMadeMove, float sourceEvaluation)
    {
        if (board.IsInCheckmate()) return _checkmate.SideFlip(whiteMadeMove);
        if (board.IsDraw()) return _draw;

        float moveEvaluation = EvaluateMove(move, whiteMadeMove);
        return sourceEvaluation + moveEvaluation;
    }

    #region Move

    private static float EvaluateMove(Move move, bool white) =>
        (EvaluateMaterial(move) + EvaluatePosition(move, white)).SideFlip(white);

    #region Material

    private static float EvaluateMaterial(Move move)
    {
        float materialScore = 0f;

        if (move.IsCapture) materialScore += GetPieceValue(move.CapturePieceType);
        if (move.IsPromotion) materialScore += GetPieceValue(move.PromotionPieceType) - 1f;

        return materialScore;
    }

    private static float GetPieceValue(PieceType pieceType) => _pieceValues[(int)pieceType];

    #endregion

    #region Position

    internal static float EvaluatePosition(Move move, bool white)
    {
        float positioning = EvaluatePositioning(move, white);

        if (move.IsCastles) positioning += _castle;

        return positioning;
    }

    #region Positioning (Valueboards)

    private static float EvaluatePositioning(Move move, bool white)
    {
        return move.MovePieceType switch
        {
            PieceType.Pawn => (white ? PawnWhiteValueBoard : PawnBlackValueBoard).EvaluateMovePosition(move),
            PieceType.Knight => KnightValueBoard.EvaluateMovePosition(move),
            PieceType.Bishop => BishopValueBoard.EvaluateMovePosition(move),
            PieceType.Queen => QueenValueBoard.EvaluateMovePosition(move),
            PieceType.King => (white ? KingWhiteValueBoard : KingBlackValueBoard).EvaluateMovePosition(move),
            _ => 0f
        };
    }

    private static float EvaluateMovePosition(this float[] valueBoard, Move move) =>
        valueBoard[move.TargetSquare.Index] - valueBoard[move.StartSquare.Index];

    #endregion

    #endregion

    #endregion

    #region Board

    internal static float EvaluateBoard(Board board)
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
        return worth.SideFlip(white);
    }

    #endregion

    private static float SideFlip(this float val, bool white) => white ? val : -val;
}