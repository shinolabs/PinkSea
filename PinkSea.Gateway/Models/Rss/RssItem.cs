using System.Xml.Serialization;

namespace PinkSea.Gateway.Models.Rss;

/// <summary>
/// The RSS 2.0 item entry.
/// </summary>
public class RssItem
{
    /// <summary>
    /// The guid of the item.
    /// </summary>
    [XmlElement("guid")]
    public required string Guid { get; set; }
    
    /// <summary>
    /// The title of the item.
    /// </summary>
    [XmlElement("title")]
    public required string Title { get; set; }
    
    /// <summary>
    /// The published date.
    /// </summary>
    [XmlElement("pubDate")]
    public required string PublishedDate { get; set; }
    
    /// <summary>
    /// The link to the item.
    /// </summary>
    [XmlElement("link")]
    public required string Link { get; set; }
    
    /// <summary>
    /// The description of the item.
    /// </summary>
    [XmlElement("description")]
    public required string Description { get; set; }
    
    /// <summary>
    /// The enclosure.
    /// </summary>
    [XmlElement("enclosure")]
    public required RssEnclosure Enclosure { get; set; }
}