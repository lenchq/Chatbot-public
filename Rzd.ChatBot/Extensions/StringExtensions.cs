namespace Rzd.ChatBot.Extensions;

public static class StringExtensions
{
    public static string Format(this string str, params object[] args)
        => string.Format(str, args);

    public static string Capitalize(this string str)
    {
        var o = char.ToUpper(str[0]);
        return o + str.ToLower().Substring(1);
    }
}