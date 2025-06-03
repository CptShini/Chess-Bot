using ChessChallenge.API;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions.TranspositionFlag;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions;

internal class TranspositionTable
{
    internal const int LookupFailed = -1;
    
    private readonly int numEntries;
    private readonly ulong _tableSize;
    
    private Board _board;
    private TranspositionEntry[] _table;

    internal TranspositionTable(int sizeMb)
    {
        int ttEntrySizeBytes = System.Runtime.InteropServices.Marshal.SizeOf<TranspositionEntry>();
        int desiredTableSizeInBytes = sizeMb * 1024 * 1024;
        numEntries = desiredTableSizeInBytes / ttEntrySizeBytes;

        _tableSize = (ulong)numEntries;
    }

    internal void Initialize(Board board)
    {
        _board = board;
        ResetTable();
    }
    
    private void ResetTable() => _table = new TranspositionEntry[numEntries];
    
    private ulong Index => _board.ZobristKey % _tableSize;

    internal Move TryGetStoredMove() => _table[Index].Move;
    
    internal int LookupEvaluation(int depth, int alpha, int beta, ref GameState gameState)
    {
        if (!TTEnabled) return LookupFailed;
        
        TranspositionEntry entry = _table[Index];
        gameState = entry.GameState;
        
        if (entry.Key != _board.ZobristKey) return LookupFailed;
        if (entry.Depth < depth) return LookupFailed;
        
        return entry.Flag switch
        {
            Exact => entry.Value,
            Alpha when entry.Value <= alpha => alpha,
            Beta when entry.Value >= beta => beta,
            _ => LookupFailed
        };
    }

    internal void StoreEvaluation(int depth, int val, TranspositionFlag flag, Move move, GameState gameState)
    {
        if (!TTEnabled) return;
        
        TranspositionEntry entry = new(_board.ZobristKey, val, depth, flag, move, gameState);
        _table[Index] = entry;
    }
}