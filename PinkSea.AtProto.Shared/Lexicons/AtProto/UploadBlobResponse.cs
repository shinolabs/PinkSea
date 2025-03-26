using System.Text.Json.Serialization;
using PinkSea.AtProto.Shared.Lexicons.Types;

namespace PinkSea.AtProto.Shared.Lexicons.AtProto;

/// <summary>
/// The response produced by "com.atproto.repo.uploadBlob".
/// </summary>
public class UploadBlobResponse
{
    [JsonPropertyName("blob")]
    public required Blob Blob { get; set; }
}