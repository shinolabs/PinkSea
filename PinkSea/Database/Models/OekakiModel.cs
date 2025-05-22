using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PinkSea.Database.Models;

/// <summary>
/// A database oekaki model.
/// </summary>
[Index(nameof(AuthorDid), nameof(OekakiTid))]
[Index(nameof(Tombstone))]
public class OekakiModel
{
    /// <summary>
    /// The key of the oekaki model.
    /// </summary>
    [Key]
    public required string Key { get; set; }
    
    /// <summary>
    /// The TID of the model.
    /// </summary>
    public required string OekakiTid { get; set; }
    
    /// <summary>
    /// The ID of the oekaki post.
    /// </summary>
    [ForeignKey(nameof(Author))]
    public required string AuthorDid { get; set; }
    
    /// <summary>
    /// When was the oekaki indexed at by the AppView?
    /// </summary>
    public required DateTimeOffset IndexedAt { get; set; }
    
    /// <summary>
    /// The author.
    /// </summary>
    public required UserModel Author { get; set; }

    /// <summary>
    /// The oekaki record cid.
    /// </summary>
    public required string RecordCid { get; set; }
    
    /// <summary>
    /// The image blob cid.
    /// </summary>
    public required string BlobCid { get; set; }
    
    /// <summary>
    /// The alt text of the image.
    /// </summary>
    public string? AltText { get; set; }
    
    /// <summary>
    /// Is this oekaki NSFW.
    /// </summary>
    public bool? IsNsfw { get; set; }
    
    /// <summary>
    /// The ID of the parent post.
    /// </summary>
    [ForeignKey(nameof(Parent))]
    public string? ParentId { get; set; }
    
    /// <summary>
    /// The ID of the parent.
    /// </summary>
    public OekakiModel? Parent { get; set; }
    
    /// <summary>
    /// Is the oekaki object a tombstone placeholder?
    /// </summary>
    public bool Tombstone { get; set; }

    /// <summary>
    /// The Bluesky crosspost record TID.
    /// </summary>
    public string? BlueskyCrosspostRecordTid { get; set; }
    
    /// <summary>
    /// The tag-oekaki relations.
    /// </summary>
    public ICollection<TagOekakiRelationModel>? TagOekakiRelations { get; set; }
}