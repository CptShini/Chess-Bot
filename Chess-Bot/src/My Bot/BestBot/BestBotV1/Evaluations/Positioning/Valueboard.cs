using System;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV1.Evaluations.Positioning;

internal static class Valueboard
{
    // TODO: Fine-tune valueboards.

    //Weight Piece Value Boards
    internal static readonly float[] PawnValueboard = CreateValueboard(Pawn, 0.25f);
    internal static readonly float[] KnightValueboard = CreateValueboard(Knight, 0.2f);
    internal static readonly float[] KingValueboard = CreateValueboard(King, 0.15f);

    //Piece Value Boards
    private static float Pawn(int x, int y) => Front(y);
    private static float Knight(int x, int y) => Center(x, y) + PunishEdges(x, y) / 2f;
    private static float King(int x, int y) => Back(y);

    // Position Formulas
    private static float Front(int y) => y / 7f;
    private static float Back(int y) => -Front(y);
    private static float Center(int i) => (3.5f - Math.Abs(i - 3.5f)) / 3f;
    private static float Center(int x, int y) => (Center(x) + Center(y)) / 2f;
    private static float PunishEdges(int i) => i is > 0 and < 7 ? 0f : -1f;
    private static float PunishEdges(int x, int y) => (PunishEdges(x) + PunishEdges(y)) / 2f;

    private static float[] CreateValueboard(Func<int, int, float> valueboardFunc, float weight)
    {
        float[] valueboard = new float[64];

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = y * 8 + x;
                valueboard[index] = valueboardFunc(x, y) * weight;
            }
        }

        return valueboard;
    }
}