using System.Xml.Serialization;

namespace PinkSea.Gateway.Models.Rss;

/// <summary>
/// The RSS 2.0 enclosure for media files.
/// </summary>
public class RssEnclosure
{
    /// <summary>
    /// The URL of the enclosure.
    /// </summary>
    [XmlAttribute("url")]
    public required string Url { get; set; }
    
    /// <summary>
    /// The size of the enclosure, in bytes.
    /// </summary>
    [XmlAttribute("length")]
    public required long Length { get; set; }
    
    /// <summary>
    /// The type of the enclosure.
    /// </summary>
    [XmlAttribute("type")]
    public required string Type { get; set; }
}