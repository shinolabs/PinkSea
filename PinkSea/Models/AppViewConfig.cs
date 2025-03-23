namespace PinkSea.Models;

/// <summary>
/// The app view config.
/// </summary>
public class AppViewConfig
{
    /// <summary>
    /// Specifies the app url.
    /// </summary>
    public required string AppUrl { get; set; }
    
    /// <summary>
    /// The source to backfill from.
    /// </summary>
    public string? BackfillSource { get; set; }
}