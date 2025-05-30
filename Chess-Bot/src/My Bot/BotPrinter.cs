using System;
using System.Text;
using ChessChallenge.API;

namespace Chess_Challenge.My_Bot;

public static class BotPrinter
{
    private static void PrintMove(this IChessBot bot, Move move, int depth, float evaluation, bool? isCheckmate = null)
    {
        StringBuilder sb = new($"{bot.GetType().Name,-11} | {move.ToString(),-13} | ");

        if (isCheckmate == true)
        {
            if (evaluation < 0) depth *= -1;
            sb.Append($"Mate in {depth}");
        }
        else
        {
            sb.Append($"Depth: {depth,2} | Evaluation: {evaluation,6:F2}"); 
        }
    
        Console.WriteLine(sb.ToString());
    }

    internal static void PrintMove(this IChessBot bot, Move move, int depth, float evaluation, float threshold) =>
        bot.PrintMove(move, depth, evaluation, Math.Abs(evaluation) > threshold);
    
    private static void PrintMove(this IChessBot bot, Move move, int depth, int evaluation, bool? isCheckmate = null) =>
        bot.PrintMove(move, depth, evaluation / 100f, isCheckmate);
    
    private static int InvertEvaluation(this int evaluation, Board board) =>
        board.IsWhiteToMove ? evaluation : -evaluation;

    internal static void PrintMove(this IChessBot bot, Board board, BestBot.BestBotV3.Evaluation.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(board), scoredMove.IsCheckmate);
    
    internal static void PrintMove(this IChessBot bot, Board board, BestBot.BestBotV4.Evaluation.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(board), scoredMove.IsCheckmate);
    
    internal static void PrintMove(this IChessBot bot, Board board, BestBot.BestBotV5.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(board), scoredMove.IsCheckmate);
    
    internal static void PrintMove(this IChessBot bot, Board board, BestBot.BestBotV6.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(board), scoredMove.IsCheckmate);
    
    internal static void PrintMove(this IChessBot bot, Board board, BestBot.BestBotV6B.ScoredMove scoredMove) =>
        bot.PrintMove(scoredMove.Move, scoredMove.Depth, scoredMove.Evaluation.InvertEvaluation(board), scoredMove.IsCheckmate);
}