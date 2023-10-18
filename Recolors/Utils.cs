namespace Recolors;

public static class Utils
{
    public static T? Random<T>(this IEnumerable<T>? input, T? defaultVal = default)
    {
        var list = input.ToList();
        return list.Count == 0 ? defaultVal : list[URandom.Range(0, list.Count)];
    }

    public static void ForEach<T>(this IEnumerable<T>? source, Action<T>? action) => source?.ToList()?.ForEach(action);

    public static string Capitalise(this string input) => input[0].ToString().ToUpper() + input.ToLower()[1..];
}