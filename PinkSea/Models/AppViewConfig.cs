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
    /// The template for the image proxy.
    /// </summary>
    public required string ImageProxyTemplate { get; set; }
    
    /// <summary>
    /// The source to backfill from.
    /// </summary>
    public string? BackfillSource { get; set; }
    
    /// <summary>
    /// Skip the dimensions verification when backfilling records.
    /// </summary>
    public bool? BackfillSkipDimensionsVerification { get; set; }
}