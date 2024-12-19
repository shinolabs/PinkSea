using System.Xml.Serialization;

namespace PinkSea.Gateway.Models.Rss;

/// <summary>
/// The RSS 2.0 root.
/// </summary>
[XmlRoot("rss")]
public class RssRoot
{
    /// <summary>
    /// The version of the RSS file.
    /// </summary>
    [XmlAttribute("version")]
    public string Version { get; set; } = "2.0";
    
    /// <summary>
    /// The RSS channel.
    /// </summary>
    [XmlElement("channel")]
    public required RssChannel Channel { get; set; }
}