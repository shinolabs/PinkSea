using System.Text.Json.Serialization;
using PinkSea.AtProto.Lexicons.Types;

namespace PinkSea.Lexicons.Records;

/// <summary>
/// The com.shinolabs.pinksea.image object.
/// </summary>
public class Image
{
    /// <summary>
    /// The com.shinolabs.pinksea.image#imageLink object.
    /// </summary>
    public class ImageLinkObject
    {
        /// <summary>
        /// Fully-qualified URL where a large version of the image can be fetched.
        /// </summary>
        [JsonPropertyName("fullsize")]
        public required string FullSize { get; set; }
        
        /// <summary>
        /// Alt text description of the image, for accessibility.
        /// </summary>
        [JsonPropertyName("alt")]
        public string? Alt { get; set; }
    }
    
    /// <summary>
    /// The actual atproto image blob.
    /// </summary>
    [JsonPropertyName("blob")]
    public required Blob Blob { get; set; }
    
    /// <summary>
    /// A link to the image, it can be either directly to the PDS or to a CDN.
    /// </summary>
    [JsonPropertyName("imageLink")]
    public required ImageLinkObject ImageLink { get; set; }
}