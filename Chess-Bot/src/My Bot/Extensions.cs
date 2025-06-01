namespace Chess_Challenge.My_Bot;

internal static class Extensions
{
    internal static int FlipIndex(this int index, bool flip = true) =>
        flip ? 63 - index : index;
    
    internal static int Perspective(this int value, bool isWhite) =>
        isWhite ? value : -value;
}