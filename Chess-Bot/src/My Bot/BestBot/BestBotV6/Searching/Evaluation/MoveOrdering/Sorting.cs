using System;
using static Chess_Challenge.My_Bot.BestBot.BestBotV6.BotSettings;

namespace Chess_Challenge.My_Bot.BestBot.BestBotV6.Searching.Evaluation.MoveOrdering;

internal static class Sorting
{
    internal static void Quicksort<T>(Span<T> values, int[] scores) =>
        Quicksort(values, scores, 0, values.Length - 1);

    private static void Quicksort<T>(Span<T> values, int[] scores, int low, int high)
    {
        while (low < high)
        {
            if (high - low <= InsertionSortThreshold)
            {
                InsertionSort(values, scores, low, high);
                break;
            }

            int pivotIndex = Partition(values, scores, low, high);
            if (pivotIndex - low < high - pivotIndex)
            {
                Quicksort(values, scores, low, pivotIndex - 1);
                low = pivotIndex + 1;
            }
            else
            {
                Quicksort(values, scores, pivotIndex + 1, high);
                high = pivotIndex - 1;
            }
        }
    }

    private static int Partition<T>(Span<T> values, int[] scores, int low, int high)
    {
        int pivotScore = scores[high];
        int i = low;

        for (int j = low; j < high; j++)
        {
            if (scores[j] <= pivotScore) continue;
            
            (values[i], values[j]) = (values[j], values[i]);
            (scores[i], scores[j]) = (scores[j], scores[i]);
            i++;
        }
        
        (values[i], values[high]) = (values[high], values[i]);
        (scores[i], scores[high]) = (scores[high], scores[i]);

        return i;
    }
    
    private static void InsertionSort<T>(Span<T> values, int[] scores, int low, int high)
    {
        for (int i = low + 1; i <= high; i++)
        {
            var key = values[i];
            int keyScore = scores[i];
            int j = i - 1;
            while (j >= low && scores[j] < keyScore)
            {
                values[j + 1] = values[j];
                scores[j + 1] = scores[j];
                j--;
            }
            values[j + 1] = key;
            scores[j + 1] = keyScore;
        }
    }
}