using System.Web;

namespace PinkSea.AtProto.Xrpc.Extensions;

/// <summary>
/// Extensions for the base object class.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Converts an object to a query string.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The resulting query string.</returns>
    public static string ToQueryString(this object obj)
    {
        var props = obj.GetType()
            .GetProperties()
            .Where(p => p.GetValue(obj) != null)
            .Select(p =>
            {
                var value = p.GetValue(obj)!.ToString();
                return $"{p.Name.ToLowerInvariant()}={HttpUtility.UrlEncode(value)}";
            });

        return string.Join('&', props);
    }
}