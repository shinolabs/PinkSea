using System.ComponentModel.DataAnnotations;

namespace PinkSea.Database.Models;

/// <summary>
/// A database oekaki model.
/// </summary>
public class OekakiModel
{
    /// <summary>
    /// The TID of the model.
    /// </summary>
    [Key]
    public required string Tid { get; set; }
    
    /// <summary>
    /// When was the oekaki indexed at by the AppView?
    /// </summary>
    public required DateTimeOffset IndexedAt { get; set; }
    
    /// <summary>
    /// The author.
    /// </summary>
    public required UserModel Author { get; set; }

    /// <summary>
    /// The image blob cid.
    /// </summary>
    public required string BlobCid { get; set; }
    
    /// <summary>
    /// The alt text of the image.
    /// </summary>
    public string? AltText { get; set; }
}