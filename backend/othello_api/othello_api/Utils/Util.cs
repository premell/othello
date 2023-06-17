public static class Utils
{
    public static Color GetOppositeColor(Color color)
    {
        return (Color)(((int)color + 1) % 2);
    }

    public static string FormatPlayerColor(Color playerColor)
    {
        return playerColor == Color.BLACK ? "Black" : "White";
    }
}











