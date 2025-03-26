using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Shared.Lexicons.Bluesky.Records;

/// <summary>
/// A facet.
/// </summary>
public class Facet
{
    /// <summary>
    /// The facet index.
    /// </summary>
    public class FacetIndex
    {
        /// <summary>
        /// The starting byte of the facet.
        /// </summary>
        [JsonPropertyName("byteStart")]
        public int ByteStart { get; set; }

        /// <summary>
        /// The ending byte of the facet.
        /// </summary>
        [JsonPropertyName("byteEnd")]
        public int ByteEnd { get; set; }

    }
    
    /// <summary>
    /// The facet's index.
    /// </summary>
    [JsonPropertyName("index")]
    public required FacetIndex Index { get; set; }
    
    /// <summary>
    /// The features of this facet.
    /// </summary>
    [JsonPropertyName("features")]
    public object[]? Features { get; set; }
}