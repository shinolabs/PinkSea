using System.Text.RegularExpressions;

namespace PinkSea.Helpers;

/// <summary>
/// A helper class for dealing with data: urls.
/// </summary>
public static partial class DataUrlHelper
{
    /// <summary>
    /// The regex for parsing data:image/ tags.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"data:image/(?<type>.+?),(?<data>.+)")]
    private static partial Regex DataRegex();
    
    /// <summary>
    /// Parses a data:image/ url into parts.
    /// </summary>
    /// <param name="dataUrl">The data url.</param>
    /// <returns>The MIME type and the byte representation.</returns>
    public static (string, byte[]) ParseDataUrl(
        string dataUrl)
    {
        var matches = DataRegex()
            .Match(dataUrl);

        var base64Data = matches.Groups["data"].Value;
        var binData = Convert.FromBase64String(base64Data);

        return (matches.Groups["type"].Value, binData);
    }
}