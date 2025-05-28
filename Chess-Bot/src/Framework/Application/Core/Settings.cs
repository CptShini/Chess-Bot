using ChessChallenge.API;
using System.Numerics;
using Chess_Challenge.My_Bot.BestBot.BestBotV1;
using Chess_Challenge.My_Bot.BestBot.BestBotV2;
using Chess_Challenge.My_Bot.BestBot.BestBotV3;
using Chess_Challenge.My_Bot.BestBot.BestBotV4;
using Chess_Challenge.My_Bot.BestBot.BestBotV5;
using Chess_Challenge.My_Bot.BestBot.BestBotV6;
using Chess_Challenge.My_Bot.MyBots;
using Chess_Challenge.My_Bot.MyBots.MyBotV1;
using Chess_Challenge.My_Bot.MyBots.MyBotV2;
using Chess_Challenge.My_Bot.MyBots.MyBotV3;
using Chess_Challenge.My_Bot.CleverBot;
using Chess_Challenge.My_Bot.CleverBot.CleverBotV1;
using Chess_Challenge.My_Bot.CleverBot.CleverBotV2;
using Chess_Challenge.My_Bot.CleverBot.CleverBotV3;
using ChessChallenge.Example;

namespace ChessChallenge.Application
{
    public static class Settings
    {
        public const string Version = "1.20";

        public static readonly IChessBot MyBot = new BestBotV6();
        public static readonly IChessBot OpponentBot = new BestBotV5();

        // Game settings
        public const int GameDurationMilliseconds = 60 * 1000;
        public const int IncrementMilliseconds = 0 * 1000;
        public const float MinMoveDelay = 0;
        public static readonly bool RunBotsOnSeparateThread = true;

        // Display settings
        public const bool DisplayBoardCoordinates = true;
        public static readonly Vector2 ScreenSizeSmall = new(1280, 720);
        public static readonly Vector2 ScreenSizeBig = new(1440, 1000);

        // Other settings
        public const int MaxTokenCount = 1024;
        public const LogType MessagesToLog = LogType.All;

        public enum LogType
        {
            None,
            ErrorOnly,
            All
        }
    }
}
