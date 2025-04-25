using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

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
    /// The cached handle of the user. May be out of date if an account event was skipped.
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// Was this user's repo deactivated?
    /// </summary>
    public UserRepoStatus RepoStatus { get; set; } = UserRepoStatus.Active;
    
    /// <summary>
    /// Was this user blocked by the admin of this AppView instance?
    /// </summary>
    public bool AppViewBlocked { get; set; }
    
    /// <summary>
    /// When was this user created at?
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Returns an expression that describes this user as not being deleted.
    /// </summary>
    [NotMapped]
    public static Expression<Func<UserModel, bool>> IsNotDeletedLinqExpression { get; } = u =>
        !(u.RepoStatus != UserRepoStatus.Active || u.AppViewBlocked);
}