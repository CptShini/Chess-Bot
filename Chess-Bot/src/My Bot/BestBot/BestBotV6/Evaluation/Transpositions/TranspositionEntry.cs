using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions;

internal readonly record struct TranspositionEntry(ulong Key, int Value, int Depth, byte Flags, Move Move)
{
    public TranspositionEntry(ulong key, int value, int depth, TranspositionFlag flag, Move move, GameState gameState)
        : this(key, value, depth, EncodeFlags(flag, gameState), move) { }
    
    public TranspositionFlag Flag => (TranspositionFlag)(Flags & 0b11);
    
    public GameState GameState => (GameState)((Flags >> 2) & 0b11);

    public override string ToString() =>
        $"Key: {Key} | Eval: {Value} | Depth: {Depth} | Flag: {Flag} | Move: {Move} | GameState: {GameState}";
    
    private static byte EncodeFlags(TranspositionFlag flag, GameState gameState) =>
        (byte)(((int)gameState << 2) | ((int)flag & 0b11));
}