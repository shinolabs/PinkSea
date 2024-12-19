using System.Xml.Serialization;
using PinkSea.Gateway.Models.Rss.Atom;

namespace PinkSea.Gateway.Models.Rss;

/// <summary>
/// An RSS 2.0 channel.
/// </summary>
public class RssChannel
{
    /// <summary>
    /// The title of the channel.
    /// </summary>
    [XmlElement("title")]
    public required string Title { get; set; }
    
    /// <summary>
    /// The link to the channel.
    /// </summary>
    [XmlElement("link")]
    public required string Link { get; set; }
    
    /// <summary>
    /// The description of the channel.
    /// </summary>
    [XmlElement("description")]
    public required string Description { get; set; }
    
    /// <summary>
    /// The atom link to this channel.
    /// </summary>
    [XmlElement("link", Namespace = "http://www.w3.org/2005/Atom")]
    public required AtomLink Atom { get; set; }
    
    /// <summary>
    /// The list of items.
    /// </summary>
    [XmlElement("item")]
    public List<RssItem>? Items { get; set; }
}