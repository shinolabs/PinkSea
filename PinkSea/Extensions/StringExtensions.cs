namespace PinkSea.Extensions;

/// <summary>
/// Extensions for the C# string class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Turns a string into a normalized tag.
    /// </summary>
    /// <param name="tag">The tag value.</param>
    /// <returns>The normalized tag.</returns>
    public static string ToNormalizedTag(this string tag)
    {
        // First normalize the tag.
        var normalized = tag.Trim();
        normalized = normalized.TrimStart('#')
            .Replace(' ', '-');

        return normalized;
    }
}