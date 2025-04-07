using Microsoft.Extensions.Options;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Lexicons.Objects;
using PinkSea.Lexicons.Queries;
using PinkSea.Models;

namespace PinkSea.Xrpc;

/// <summary>
/// [UNSPECCED - DO NOT USE]
/// Handler for the "com.shinolabs.pinksea.unspecced.getProfile" query. Gets the profile information for a DID.
/// </summary>
[Xrpc("com.shinolabs.pinksea.unspecced.getProfile")]
public class GetProfileQueryHandler(
    IDidResolver didResolver,
    IOptions<AppViewConfig> opts) : IXrpcQuery<GetProfileQueryRequest, GetProfileQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetProfileQueryResponse>> Handle(
        GetProfileQueryRequest request)
    {
        var handle = await didResolver.GetHandleFromDid(request.Did);
        if (string.IsNullOrEmpty(handle))
        {
            return XrpcErrorOr<GetProfileQueryResponse>.Fail(
                "ProfileDoesntExist",
                "This profile does not exist.");
        }

        // TODO: This is only a mock for iOSSea before we implement profile editing for realsies.
        //       Don't mind me being here!
        var profile = new GetProfileQueryResponse
        {
            Did = request.Did,
            Handle = handle,
            Nickname = handle,
            Description = "",
            Links =
            [
                new Link { Name = "Bluesky", Url = $"https://bsky.app/profile/{request.Did}" },
                new Link { Name = "Website", Url = $"https://{handle}" }
            ],
            Avatar = $"{opts.Value.AppUrl}/blank_avatar.png"
        };
        
        return XrpcErrorOr<GetProfileQueryResponse>.Ok(profile);
    }
}