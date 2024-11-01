using System.ComponentModel.DataAnnotations;

namespace PinkSea.Database.Models;

/// <summary>
/// The database model for the user.
/// </summary>
public class UserModel
{
    /// <summary>
    /// The DID of the user.
    /// </summary>
    [Key]
    public required string Did { get; set; }
    
    /// <summary>
    /// When was this user created at?
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }
}