using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation;

internal class TranspositionTable
{
    internal const int LookupFailed = -1;
    internal const int FlagExact = 0;
    internal const int FlagAlpha = 1;
    internal const int FlagBeta = 2;

    private readonly Board _board;
    
    private readonly ulong _tableSize;
    private readonly TranspositionEntry[] _table;
    
    private const bool _enabled = true;

    public TranspositionTable(Board board, int sizeMb)
    {
        _board = board;

        int ttEntrySizeBytes = System.Runtime.InteropServices.Marshal.SizeOf<TranspositionEntry>();
        int desiredTableSizeInBytes = sizeMb * 1024 * 1024;
        int numEntries = desiredTableSizeInBytes / ttEntrySizeBytes;

        _tableSize = (ulong)numEntries;
        _table = new TranspositionEntry[numEntries];
    }

    private ulong Index => _board.ZobristKey % _tableSize;

    internal int LookupEvaluation(int depth, int alpha, int beta)
    {
        if (!_enabled) return LookupFailed;
        
        TranspositionEntry entry = _table[Index];
        
        if (entry.Key != _board.ZobristKey) return LookupFailed;
        if (entry.Depth < depth) return LookupFailed;
        
        return entry.Flag switch
        {
            FlagExact => entry.Value,
            FlagAlpha when entry.Value <= alpha => alpha,
            FlagBeta when entry.Value >= beta => beta,
            _ => LookupFailed
        };
    }

    internal void StoreEvaluation(int depth, int val, int flags/*, Move move*/)
    {
        if (!_enabled) return;
        
        TranspositionEntry entry = new(_board.ZobristKey, val, depth, flags, Move.NullMove);
        _table[Index] = entry;
    }

    internal float GetStorageFullness()
    {
        int filledEntries = _table.Count(e => e.Key != 0);
        return (float)filledEntries / _tableSize;
    }
}