namespace PinkSea.AtProto.Helpers;

/// <summary>
/// Utilities for working with at:// URIs used by the AT Protocol.
/// </summary>
public class AtLinkHelper
{
    private const string SchemePrefix = "at://";

    /// <summary>Strongly-typed view of an AT Proto URI.</summary>
    public readonly record struct AtUri(string Authority, string Collection, string RecordKey)
    {
        public override string ToString() => $"{SchemePrefix}{Authority}/{Collection}/{RecordKey}";
    }

    /// <summary>
    /// Parse a raw <c>at://</c> string and throw if it is invalid.
    /// </summary>
    public static AtUri Parse(string value)
    {
        if (!TryParse(value, out var result))
            throw new FormatException($"Invalid AT URI: \"{value}\"");
        return result;
    }

    /// <summary>
    /// Try-parse variant that never throws.
    /// </summary>
    public static bool TryParse(string value, out AtUri result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value) ||
            !value.StartsWith(SchemePrefix, StringComparison.OrdinalIgnoreCase))
            return false;

        // work with spans to avoid allocations
        ReadOnlySpan<char> span = value.AsSpan(SchemePrefix.Length);

        // authority = everything up to first '/'
        int firstSlash = span.IndexOf('/');
        if (firstSlash <= 0) return false;
        var authority = span[..firstSlash].ToString();

        span = span[(firstSlash + 1)..];          // skip first '/'
        int secondSlash = span.IndexOf('/');
        if (secondSlash <= 0 || secondSlash == span.Length - 1) return false;

        var collection = span[..secondSlash].ToString();
        var recordKey  = span[(secondSlash + 1)..].ToString();

        // basic sanity
        if (collection.Length == 0 || recordKey.Length == 0)
            return false;

        result = new AtUri(authority, collection, recordKey);
        return true;
    }
}