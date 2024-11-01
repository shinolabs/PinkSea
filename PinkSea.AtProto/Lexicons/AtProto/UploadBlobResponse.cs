using System.Text.Json.Serialization;

namespace PinkSea.AtProto.Lexicons.AtProto;

/// <summary>
/// The response produced by "com.atproto.repo.uploadBlob".
/// </summary>
public class UploadBlobResponse
{
    [JsonPropertyName("blob")]
    public required string Blob { get; set; }
}