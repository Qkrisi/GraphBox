using Blazor.Diagrams.Core.Models;

namespace GraphUtil;

public static class Extensions
{
    public static void SetColor(this LinkModel link, string? color)
    {
        link.Color = color;
        link.Refresh();
    }
    
    public static string ReplaceFirst(this string str, string term, string replace)
    {
        int position = str.IndexOf(term);
        if (position < 0)
        {
            return str;
        }
        str = str.Substring(0, position) + replace + str.Substring(position + term.Length);
        return str;
    }

    public static string GetText<T>(this IEnumerable<T> enumerable, bool reverse = false)
    {
        var strings = enumerable.Select(element => element?.ToString());
        if (reverse)
            strings = strings.Reverse();
        return string.Join("; ", strings);
    }
}