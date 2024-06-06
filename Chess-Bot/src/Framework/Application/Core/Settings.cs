using ChessChallenge.API;
using System.Numerics;
using Chess_Challenge.My_Bot.BestBot.BestBotV2;
using Chess_Challenge.My_Bot.BestBot.BestBotV3;
using Chess_Challenge.My_Bot.BestBot.BestBotV4;

namespace ChessChallenge.Application
{
    public static class Settings
    {
        public const string Version = "1.20";

        public static readonly IChessBot MyBot = new BestBotV4();
        public static readonly IChessBot OpponentBot = new BestBotV3();

        // Game settings
        public const int GameDurationMilliseconds = 60 * 1000;
        public const int IncrementMilliseconds = 0 * 1000;
        public const float MinMoveDelay = 0;
        public static readonly bool RunBotsOnSeparateThread = true;

        // Display settings
        public const bool DisplayBoardCoordinates = true;
        public static readonly Vector2 ScreenSizeSmall = new(1280, 720);
        public static readonly Vector2 ScreenSizeBig = new(1920, 1080);

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
