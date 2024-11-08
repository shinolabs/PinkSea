using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinkSea.Database.Models;

/// <summary>
/// A relation between a tag and an oekaki post.
/// </summary>
public class TagOekakiRelationModel
{
    /// <summary>
    /// The key of the relation.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// The ID of the oekaki post.
    /// </summary>
    [ForeignKey(nameof(Oekaki))]
    public required string OekakiId { get; set; }
    
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    [ForeignKey(nameof(Tag))]
    public required string TagId { get; set; }
    
    /// <summary>
    /// The ID of the oekaki post.
    /// </summary>
    public required OekakiModel Oekaki { get; set; }
    
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    public required TagModel Tag { get; set; }
}