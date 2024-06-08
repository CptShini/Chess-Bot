using System;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV4.Evaluation;

internal class TranspositionTable
{
    internal const int LookupFailed = -1;
    internal const int FlagExact = 0;
    internal const int FlagAlpha = 1;
    internal const int FlagBeta = 2;

    private readonly TranspositionEntry[] _entries;

    private readonly ulong _count;
    private readonly bool _enabled = true;
    private readonly Board _board;

    public TranspositionTable(Board board, int sizeMb)
    {
        _board = board;

        int ttEntrySizeBytes = System.Runtime.InteropServices.Marshal.SizeOf<TranspositionEntry>();
        int desiredTableSizeInBytes = sizeMb * 1024 * 1024;
        int numEntries = desiredTableSizeInBytes / ttEntrySizeBytes;

        _count = (ulong)numEntries;
        _entries = new TranspositionEntry[numEntries];
    }

    private ulong Index => _board.ZobristKey % _count;

    internal int LookupEvaluation(int depth, int alpha, int beta)
    {
        if (!_enabled) return LookupFailed;
        
        TranspositionEntry entry = _entries[Index];
        
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
        
        TranspositionEntry entry = new(_board.ZobristKey, val, Move.NullMove, depth, flags);
        _entries[Index] = entry;
        //Console.WriteLine(entry);
    }

}