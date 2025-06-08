using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Material;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.MoveOrdering;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.GameState;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation;

internal class BoardWrapper
{
    private readonly Board _board;
    
    private protected bool IsWhiteToMove { get; private set; }
    
    private int EnemyPieceCount => IsWhiteToMove ? _blackPieceCount : _whitePieceCount;
    private int _whitePieceCount, _blackPieceCount;
    
    private protected BoardWrapper(Board board)
    {
        _board = board;
        IsWhiteToMove = board.IsWhiteToMove;

        _whitePieceCount = CountPieces(true);
        _blackPieceCount = CountPieces(false);

        return;
        
        int CountPieces(bool white)
        {
            ulong bitBoard = white ? board.WhitePiecesBitboard : board.BlackPiecesBitboard;
            return System.Numerics.BitOperations.PopCount(bitBoard);
        }
    }
    
    internal int EvaluateMove(Move move) =>
        move.EvaluateMaterial() +
        move.EvaluatePositioning(IsWhiteToMove, EnemyPieceCount);

    internal int EvaluateBoard() =>
        _board.EvaluateMaterial() + 
        _board.EvaluatePositioning(EnemyPieceCount);
    
    internal void GetOrderedMoves(ref Span<Move> moves, bool capturesOnly, Move pvMove)
    {
        _board.GetLegalMovesNonAlloc(ref moves, capturesOnly);
        _board.OrderMoves(moves, pvMove, EnemyPieceCount);
    }
    
    internal GameState EvaluateGameState(int plyFromRoot, out int endEvaluation)
    {
        bool isCheckmate = _board.IsInCheckmate();
        if (isCheckmate)
        {
            endEvaluation = CheckmateValue + plyFromRoot;
            return Checkmate;
        }

        bool isDraw = _board.IsDraw();
        if (isDraw)
        {
            float earlyGameFactor = (EnemyPieceCount - 1) / 15f;
            int drawValue = (int)(earlyGameFactor * -ContemptValue);
            endEvaluation = drawValue;
            return Draw;
        }

        endEvaluation = 0;
        return GameNotOver;
    }
    
    internal void MakeMove(Move move)
    {
        OnMakeMove(move);
        
        _board.MakeMove(move);
        IsWhiteToMove = !IsWhiteToMove;
    }
    
    internal void UndoMove(Move move)
    {
        _board.UndoMove(move);
        IsWhiteToMove = !IsWhiteToMove;

        OnUndoMove(move);
    }
    
    private protected virtual void OnMakeMove(Move move)
    {
        if (move.IsCapture)
        {
            if (IsWhiteToMove) _blackPieceCount--;
            else _whitePieceCount--;
        }
    }
    
    private protected virtual void OnUndoMove(Move move)
    {
        if (move.IsCapture)
        {
            if (IsWhiteToMove) _blackPieceCount++;
            else _whitePieceCount++;
        }
    }
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        
        string side = IsWhiteToMove ? "White" : "Black";
        sb.AppendLine($"{side} to move");
        
        sb.Append($"W:{_whitePieceCount,2} ");
        sb.Append($"B:{_blackPieceCount,2} ");
        sb.Append($"(Enemy: {EnemyPieceCount,2})");
        sb.AppendLine();

        sb.Append(_board);

        return sb.ToString();
    }
}