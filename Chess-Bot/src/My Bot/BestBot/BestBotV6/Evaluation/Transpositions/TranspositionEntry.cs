using ChessChallenge.API;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Evaluation.Transpositions;

internal readonly record struct TranspositionEntry(ulong Key, int Value, int Depth, byte Flags, Move Move)
{
    public TranspositionFlag Flag => (TranspositionFlag)(Flags & 0b11);
    public GameState GameState => (GameState)((Flags >> 2) & 0b11);
    
    public TranspositionEntry(ulong key, int value, int depth, TranspositionFlag flag, Move move, GameState gameState)
        : this(key, value, depth, EncodeFlags(flag, gameState), move) { }
    
    private static byte EncodeFlags(TranspositionFlag flag, GameState gameState) =>
        (byte)(((int)gameState << 2) | ((int)flag & 0b11));
    
    public override string ToString() =>
        $"Key: 0x{Key:X16}, Value: {Value,6:+0;-0;0}, Depth: {Depth,2}, Flag: {Flag}, State: {GameState}, {Move}";
}