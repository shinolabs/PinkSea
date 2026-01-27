using PinkSea.Database.Models;

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
            .Replace(' ', '_');

        return normalized;
    }

    /// <summary>
    /// Maps an ATProto repo status to a PinkSea repo status.
    /// </summary>
    /// <param name="status">The repo status string.</param>
    /// <returns>The UserRepoStatus.</returns>
    public static UserRepoStatus ToRepoStatus(this string status)
    {
        return status switch
        {
            "takendown" => UserRepoStatus.TakenDown,
            "suspended" => UserRepoStatus.Suspended,
            "deactivated" => UserRepoStatus.Deactivated,
            "deleted" => UserRepoStatus.Deleted,
            _ => UserRepoStatus.Unknown
        };
    }

    /// <summary>
    /// Ensures that a given string is an HTTP url.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <returns>The formatted URL to be an HTTP url.</returns>
    public static string EnsureHttpUrl(this string url)
    {
        if (url.StartsWith("http"))
            return url;

        return $"https://{url}";
    }
}