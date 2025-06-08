using System.Collections.Generic;
using System.Linq;

namespace Chess_Challenge.My_Bot;

public static class Random
{
    private static readonly System.Random _rng = new();
    
    public static int Next(int maxValue) => _rng.Next(maxValue);
    public static int Next(int minValue, int maxValue) => _rng.Next(minValue, maxValue);
    public static T RandomElement<T>(this T[] elements) => elements[_rng.Next(elements.Length)];
    
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = _rng.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}