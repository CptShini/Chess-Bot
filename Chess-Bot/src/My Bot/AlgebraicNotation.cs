using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot;

internal static class AlgebraicNotation
{
    internal static string GetAlgebraicNotation(this Move move)
    {
        return move.IsCastles ?
            move.CastleString() :
            $"{move.MoveSymbol()}{move.CaptureSymbol()}{move.TargetSquare.Name}{move.PromotionSuffix()}";
    }

    private static string CastleString(this Move move)
    {
        return (move.StartSquare.Name, move.TargetSquare.Name) switch
        {
            ("e1", "g1") or ("e8", "g8") => "O-O",
            ("e1", "c1") or ("e8", "c8") => "O-O-O",
            _ => throw new InvalidOperationException("Invalid castling move")
        };
    }
    
    private static string PieceToLetter(this PieceType piece) =>
        piece switch
        {
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            PieceType.Rook => "R",
            PieceType.Queen => "Q",
            PieceType.King => "K",
            _ => ""
        };

    private static string MoveSymbol(this Move move)
    {
        string pieceLetter = PieceToLetter(move.MovePieceType);
        
        // If move is pawn capture
        if (move is { MovePieceType: PieceType.Pawn, IsCapture: true })
        {
            pieceLetter = move.StartSquare.Name[0].ToString();
        }

        return pieceLetter;
    }

    private static string CaptureSymbol(this Move move) =>
        move.IsCapture ? "x" : "";

    private static string PromotionSuffix(this Move move) =>
        move.IsPromotion ? $"={PieceToLetter(move.PromotionPieceType)}" : "";
}