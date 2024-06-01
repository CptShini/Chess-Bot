using System;

namespace CleverBot.CleverBotV3.Evaluations;

internal static class Valueboard
{
    internal readonly static float[] PawnWhiteValueBoard = CreateValueBoard(PawnPositionalValue);
    internal readonly static float[] PawnBlackValueBoard = CreateValueBoard(PawnPositionalValue, true);
    internal readonly static float[] KnightValueBoard = CreateValueBoard(KnightPositionalValue);
    internal readonly static float[] BishopValueBoard = CreateValueBoard(BishopPositionalValue);
    internal readonly static float[] QueenValueBoard = CreateValueBoard(QueenPositionalValue);
    internal readonly static float[] KingWhiteValueBoard = CreateValueBoard(KingPositionalValue);
    internal readonly static float[] KingBlackValueBoard = CreateValueBoard(KingPositionalValue, true);

    private static float PawnPositionalValue(int x, int y) => Front(y) * 0.15f;
    private static float KnightPositionalValue(int x, int y) => (Center(x) + Center(y)) * 0.05f;
    private static float BishopPositionalValue(int x, int y) => (Inner(x) + Inner(y)) * 0.15f;
    private static float QueenPositionalValue(int x, int y) => (Inner(x) + Inner(y)) * 0.1f;
    private static float KingPositionalValue(int x, int y) => Back(y) * 0.2f;

    private static float Front(int x) => x * 0.15f;
    private static float Back(int x) => -x * 0.15f;
    private static float Center(int x) => (3.5f - Math.Abs(x - 3.5f)) * 0.333f;
    private static float Inner(int x) => x is > 0 and < 7 ? 0f : -1f;

    private static float[] CreateValueBoard(Func<int, int, float> valueBoardFunc, bool inverse = false)
    {
        float[] valueBoard = new float[64];

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = inverse ? 63 - (y * 8 + x) : y * 8 + x;
                valueBoard[index] = valueBoardFunc(x, y);
            }
        }

        return valueBoard;
    }
}