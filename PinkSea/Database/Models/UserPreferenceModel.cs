using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinkSea.Database.Models;

/// <summary>
/// A singular preference for a given user.
/// </summary>
public class UserPreferenceModel
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

    public required string Key { get; set; }

    public required string Value { get; set; }
}