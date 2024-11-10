namespace PinkSea.Models;

/// <summary>
/// The postgres config.
/// </summary>
public class PostgresConfig
{
    /// <summary>
    /// The hostname.
    /// </summary>
    public required string Hostname { get; set; }
    
    /// <summary>
    /// The post.
    /// </summary>
    public required int Port { get; set; }
    
    /// <summary>
    /// The username.
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// The password.
    /// </summary>
    public required string Password { get; set; }
    
    /// <summary>
    /// The database.
    /// </summary>
    public required string Database { get; set; }
}