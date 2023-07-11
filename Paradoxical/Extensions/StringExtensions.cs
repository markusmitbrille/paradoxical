namespace Paradoxical.Extensions;

public static class StringExtensions
{
    public static bool IsEmpty(this string obj)
    {
        return obj.Length == 0;
    }
}
