using System.ComponentModel.DataAnnotations;

namespace PinkSea.Database.Models;

/// <summary>
/// The tag database model.
/// </summary>
public class TagModel
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    [Key]
    public required string Name { get; set; }
}