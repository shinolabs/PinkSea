namespace PinkSea.AtProto.Models.Authorization;

/// <summary>
/// The current authorization type.
/// </summary>
public enum AuthorizationType
{
    /// <summary>
    /// A session with the PDS.
    /// </summary>
    PdsSession,
    
    /// <summary>
    /// OAuth2.
    /// </summary>
    OAuth2
}