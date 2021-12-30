namespace Shared.Extensions;

public static class ExtensionMethods
{
    public static int ToInt(this float number) => (int)Math.Round(number);
    public static int ToInt(this string number, int fromBase = 10) => Convert.ToInt32(number, fromBase);
}
