using PinkSea.Database.Models;

namespace PinkSea.Models.Oekaki;

/// <summary>
/// The oekaki upload result.
/// </summary>
/// <param name="State">The state of the upload.</param>
/// <param name="Oekaki">The oekaki model.</param>
public record OekakiUploadResult(
    OekakiUploadState State,
    OekakiModel? Oekaki = null);