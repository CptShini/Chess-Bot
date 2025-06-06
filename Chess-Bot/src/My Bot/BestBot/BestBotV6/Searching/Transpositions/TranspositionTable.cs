using System;
using System.Linq;
using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Transpositions.TranspositionFlag;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Transpositions;

internal class TranspositionTable
{
    internal const int LookupFailed = int.MinValue;
    
    private readonly ulong _tableSize;
    private readonly TranspositionEntry[] _table;
    
    private Board _board;

    internal TranspositionTable(int sizeMb)
    {
        int ttEntrySizeBytes = System.Runtime.InteropServices.Marshal.SizeOf<TranspositionEntry>();
        int desiredTableSizeInBytes = sizeMb * 1024 * 1024;
        int numEntries = desiredTableSizeInBytes / ttEntrySizeBytes;

        _tableSize = (ulong)numEntries;
        _table = new TranspositionEntry[numEntries];
    }

    internal void Initialize(Board board)
    {
        _board = board;
        Array.Clear(_table, 0, _table.Length);
    }
    
    private ulong Index => _board.ZobristKey % _tableSize;

    internal Move TryGetStoredMove() => _table[Index].Move;
    
    internal int LookupEvaluation(int depth, int alpha, int beta)
    {
        if (!TTEnabled) return LookupFailed;
        
        TranspositionEntry entry = _table[Index];
        if (entry.Key != _board.ZobristKey) return LookupFailed;
        if (entry.Depth < depth) return LookupFailed;
        
        return entry.Flag switch
        {
            Exact => entry.Value,
            Alpha when entry.Value <= alpha => entry.Value,
            Beta when entry.Value >= beta => entry.Value,
            _ => LookupFailed
        };
    }

    internal void StoreEvaluation(int depth, int val, TranspositionFlag flag, Move move)
    {
        if (!TTEnabled) return;
        
        TranspositionEntry entry = new(_board.ZobristKey, val, depth, flag, move);
        _table[Index] = entry;
    }
    
    public override string ToString()
    {
        int filled = _table.Count(e => e.Key != 0);
        double fillPercent = 100f * filled / _tableSize;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Transposition Table");
        sb.AppendLine($"Entries     : {_tableSize:N0}");
        sb.AppendLine($"Filled      : {filled:N0} ({fillPercent:F2}%)");

        return sb.ToString();
    }

}