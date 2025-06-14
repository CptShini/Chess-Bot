﻿using System;
using Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.Positioning.Valueboards;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6;

public static class BotSettings
{
    // Piece values
    internal const int
        PawnValue = 1_00,
        KnightValue = 3_05,
        BishopValue = 3_33,
        RookValue = 5_63,
        QueenValue = 9_50,
        KingValue = 100_00;
    
    // State & move values
    internal const int
        CheckmateValue = -KingValue,
        ContemptValue = -0_50,
        CastleValue = 0_50,
        MopUpClosenessFactor = 5,
        MaxExtensions = 16;

    // Move ordering
    private const int million = 1_000_000;
    internal const int
        pvMoveBias = 100 * million,
        winningCaptureBias = 8 * million,
        promoteBias = 6 * million,
        losingCaptureBias = 2 * million;
    internal const int InsertionSortThreshold = 12;

    // Transposition table
    internal const bool TTEnabled = true;
    internal const int
        TTSize = 64;

    // Thinker
    internal const int DepthLimit = 48;
    internal const float MaxThinkTimeFactor = 1.5f;
    internal static float TurnThinkTime(int msRemaining) =>
        msRemaining switch
        {
            < 10_000 => msRemaining / 30f,
            < 60_000 => msRemaining / 25f - 66.666f,
            _ => MathF.Pow(MathF.Sqrt(msRemaining), 1.5f) / 1.643f
        };

    // Think time estimation
    internal const int TableLength = 5;
    internal static float BranchFactorRecencyWeight(int n) => 1f / (n + 1);
    
    // Valueboards
    internal static readonly Valueboard
        PawnValueboard = new(
            new(
                [
                    0, 0, 0, 0, 0, 0, 0, 0,
                    50, 50, 50, 50, 50, 50, 50, 50,
                    10, 10, 20, 30, 30, 20, 10, 10,
                    5, 5, 10, 25, 25, 10, 5, 5,
                    0, 0, 0, 20, 20, 0, 0, 0,
                    5, -5, -10, 0, 0, -10, -5, 5,
                    5, 10, 10, -20, -20, 10, 10, 5,
                    0, 0, 0, 0, 0, 0, 0, 0
                ],
                1.2f
            ),
            new WeightedValueboard(
                CreateValueboard(Pawn),
                1f,
                false
            )
        ),
        KnightValueboard = new(
            new(
                [
                    -50, -40, -30, -30, -30, -30, -40, -50,
                    -40, -20, 0, 0, 0, 0, -20, -40,
                    -30, 0, 10, 15, 15, 10, 0, -30,
                    -30, 5, 15, 20, 20, 15, 5, -30,
                    -30, 0, 15, 20, 20, 15, 0, -30,
                    -30, 5, 10, 15, 15, 10, 5, -30,
                    -40, -20, 0, 5, 5, 0, -20, -40,
                    -50, -40, -30, -30, -30, -30, -40, -50
                ],
                0.8f
            ),
            0.4f
        ),
        BishopValueboard = new(
            new(
                [
                    -20, -10, -10, -10, -10, -10, -10, -20,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -10, 0, 5, 10, 10, 5, 0, -10,
                    -10, 5, 5, 10, 10, 5, 5, -10,
                    -10, 0, 10, 10, 10, 10, 0, -10,
                    -10, 10, 10, 10, 10, 10, 10, -10,
                    -10, 5, 0, 0, 0, 0, 5, -10,
                    -20, -10, -10, -10, -10, -10, -10, -20
                ],
                1f
            ),
            0.5f
        ),
        RookValueboard = new(
            new(
                [
                    0, 0, 0, 0, 0, 0, 0, 0,
                    5, 10, 10, 10, 10, 10, 10, 5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    0, 0, 0, 5, 5, 0, 0, 0
                ],
                1f
            ),
            0f
        ),
        QueenValueboard = new(
            new(
                [
                    -20, -10, -10, -5, -5, -10, -10, -20,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -10, 0, 5, 5, 5, 5, 0, -10,
                    -5, 0, 5, 5, 5, 5, 0, -5,
                    0, 0, 5, 5, 5, 5, 0, -5,
                    -10, 5, 5, 5, 5, 5, 0, -10,
                    -10, 0, 5, 0, 0, 0, 0, -10,
                    -20, -10, -10, -5, -5, -10, -10, -20
                ],
                1f
            ),
            0.4f
        ),
        KingValueboard = new(
            new(
                [
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -20, -30, -30, -40, -40, -30, -30, -20,
                    -10, -20, -20, -20, -20, -20, -20, -10,
                    20, 20, 0, 0, 0, 0, 20, 20,
                    20, 30, 10, 0, 0, 10, 30, 20
                ],
                1.2f
            ),
            new WeightedValueboard(
                [
                    -50, -40, -30, -20, -20, -30, -40, -50,
                    -30, -20, -10, 0, 0, -10, -20, -30,
                    -30, -10, 20, 30, 30, 20, -10, -30,
                    -30, -10, 30, 40, 40, 30, -10, -30,
                    -30, -10, 30, 40, 40, 30, -10, -30,
                    -30, -10, 20, 30, 30, 20, -10, -30,
                    -30, -30, 0, 0, 0, 0, -30, -30,
                    -50, -30, -30, -30, -30, -30, -30, -50
                ],
                0.8f
            )
        );

    //Piece Value Boards
    private static float Pawn(int x, int y) => Front(y);
    private static float Knight(int x, int y) => Center(x, y) + PunishEdges(x, y) / 2f;

    // Position Formulas
    private static float Front(int y) => y / 7f;
    private static float Back(int y) => -Front(y);
    private static float Center(int i) => (3.5f - Math.Abs(i - 3.5f)) / 3f;
    private static float Center(int x, int y) => (Center(x) + Center(y)) / 2f;
    private static float PunishEdges(int i) => i is > 0 and < 7 ? 0f : -1f;
    private static float PunishEdges(int x, int y) => (PunishEdges(x) + PunishEdges(y)) / 2f;

    private static float[] CreateValueboard(Func<int, int, float> valueboardFunc)
    {
        float[] valueboard = new float[64];

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = y * 8 + x;
                valueboard[index] = valueboardFunc(x, y) * 100f;
            }
        }

        return valueboard;
    }
}