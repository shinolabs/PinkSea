using System.ComponentModel.DataAnnotations;

namespace PinkSea.Database.Models;

/// <summary>
/// The OAuth state database model.
/// </summary>
public class OAuthStateModel
{
    /// <summary>
    /// The id of the oauth state.
    /// </summary>
    [Key]
    public required string Id { get; set; }
    
    /// <summary>
    /// TEMPORARY, the json of the oauth state model.
    /// </summary>
    public required string Json { get; set; }
}