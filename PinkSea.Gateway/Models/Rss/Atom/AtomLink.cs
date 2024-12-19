using System.Xml.Serialization;

namespace PinkSea.Gateway.Models.Rss.Atom;

/// <summary>
/// An Atom link.
/// </summary>
public class AtomLink
{
    /// <summary>
    /// The link.
    /// </summary>
    [XmlAttribute("href")]
    public required string Link { get; set; }
    
    /// <summary>
    /// The rel attribute.
    /// </summary>
    [XmlAttribute("rel")]
    public required string Relative { get; set; }
    
    /// <summary>
    /// The type of the link.
    /// </summary>
    [XmlAttribute("type")]
    public required string Type { get; set; }
}