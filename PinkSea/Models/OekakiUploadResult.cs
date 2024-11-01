namespace PinkSea.Models;

/// <summary>
/// The upload result.
/// </summary>
public enum OekakiUploadResult
{
    /// <summary>
    /// The user is not authorized.
    /// </summary>
    NotAuthorized,
    
    /// <summary>
    /// Not a png file.
    /// </summary>
    NotAPng,
    
    /// <summary>
    /// The upload was too big.
    /// </summary>
    UploadTooBig,
    
    /// <summary>
    /// Failed to upload the blob.
    /// </summary>
    FailedToUploadBlob,
    
    /// <summary>
    /// Failed to upload the record.
    /// </summary>
    FailedToUploadRecord,
    
    /// <summary>
    /// Everything is a-okay.
    /// </summary>
    Ok
}