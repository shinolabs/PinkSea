using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinkSea.Database.Models;

/// <summary>
/// The configuration database model.
/// </summary>
public class ConfigurationModel
{
    /// <summary>
    /// The id.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// The private key of the client.
    /// </summary>
    public required string ClientPrivateKey { get; set; }
    
    /// <summary>
    /// The private key of the client.
    /// </summary>
    public required string ClientPublicKey { get; set; }
    
    /// <summary>
    /// The key id of the JWK.
    /// </summary>
    public required string KeyId { get; set; }
    
    /// <summary>
    /// Have we synchronized the account states?
    /// </summary>
    public bool? SynchronizedAccountStates { get; set; }
    
    /// <summary>
    /// Have we imported profiles?
    /// </summary>
    public bool? ImportedProfiles { get; set; }
}