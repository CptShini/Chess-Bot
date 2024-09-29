using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV5.Evaluation.Transpositions;

internal class TranspositionTable
{
    internal const int LookupFailed = -1;
    internal const int FlagExact = 0;
    internal const int FlagAlpha = 1;
    internal const int FlagBeta = 2;

    private Board _board;
    
    private readonly ulong _tableSize;
    private readonly TranspositionEntry[] _table;
    
    private const bool _enabled = true;

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
        for (int i = 0; i < (int)_tableSize; i++)
        {
            if (_table[i].Key == 0) continue;
            
            _table[i] = new();
        }
    }
    
    private ulong Index => _board.ZobristKey % _tableSize;

    internal Move TryGetStoredMove() => _table[Index].Move;
    
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

    internal void StoreEvaluation(int depth, int val, int flags, Move move)
    {
        if (!_enabled) return;
        
        TranspositionEntry entry = new(_board.ZobristKey, val, depth, flags, move);
        _table[Index] = entry;
    }
}