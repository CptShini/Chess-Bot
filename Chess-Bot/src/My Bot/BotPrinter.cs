using System;
using System.Text;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot;

public static class BotPrinter
{
    private static void PrintMove(this IChessBot bot, Move move, int depth, float evaluation, int gameState)
    {
        StringBuilder sb = new();
        AppendMoveString();
        Console.WriteLine(sb.ToString());

        return;
        
        void AppendMoveString()
        {
            sb.Append($"{bot.GetType().Name,-11}");
            sb.Append($" | {move.ToString(),-13}");
            if (move.IsNull) return;
        
            sb.Append($" | {move.GetAlgebraicNotation(),-6}");
        
            switch (gameState)
            {
                case 0:
                    sb.Append($" | Depth: {depth,2}");
                    sb.Append($" | Evaluation: {evaluation,6:F2}");
                    break;
                case 1:
                    sb.Append($" | Draw in {depth,2}");
                    break;
                case 2:
                    if (evaluation < 0) depth *= -1;
                    sb.Append($" | Mate in {depth,3}");
                    break;
            }
        }
    }

    internal static void PrintMove(this IChessBot bot, Move move, int depth, float evaluation, float threshold) =>
        bot.PrintMove(move, depth, evaluation, Math.Abs(evaluation) > threshold ? 1 : -1);
    
    private static void PrintMove(this IChessBot bot, Move move, int depth, float evaluation, bool isCheckmate) =>
        bot.PrintMove(move, depth, evaluation, isCheckmate ? 2 : 0);
    
    private static int InvertEvaluation(this int evaluation, bool isWhiteToMove) =>
        isWhiteToMove ? evaluation : -evaluation;

    internal static void PrintMove(this IChessBot bot, bool isWhiteToMove, BestBot.BestBotV3.Evaluation.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(isWhiteToMove) / 100f, scoredMove.IsCheckmate);
    
    internal static void PrintMove(this IChessBot bot, bool isWhiteToMove, BestBot.BestBotV4.Evaluation.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(isWhiteToMove) / 100f, scoredMove.IsCheckmate);
    
    internal static void PrintMove(this IChessBot bot, bool isWhiteToMove, BestBot.BestBotV5.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(isWhiteToMove) / 100f, scoredMove.IsCheckmate);

    internal static void PrintMove(this IChessBot bot, bool isWhiteToMove, BestBot.BestBotV6.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(isWhiteToMove) / 100f, (int)scoredMove.GameState);
}