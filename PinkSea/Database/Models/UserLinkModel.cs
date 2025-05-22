using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinkSea.Database.Models;

/// <summary>
/// A link on a user's page.
/// </summary>
public class UserLinkModel
{
    /// <summary>
    /// The key of the link.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// The DID of the user that owns this link.
    /// </summary>
    [ForeignKey(nameof(User))]
    public required string UserDid { get; set; }
    
    /// <summary>
    /// The user that owns this link.
    /// </summary>
    public required UserModel User { get; set; }
    
    /// <summary>
    /// The name of the link.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The URL of the link.
    /// </summary>
    public required string Url { get; set; }
}