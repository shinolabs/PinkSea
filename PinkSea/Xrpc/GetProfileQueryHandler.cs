using Microsoft.Extensions.Options;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Lexicons.Types;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Database.Models;
using PinkSea.Lexicons.Objects;
using PinkSea.Lexicons.Queries;
using PinkSea.Models;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getProfile" query. Gets the profile information for a DID.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getProfile")]
public class GetProfileQueryHandler(
    UserService userService,
    IDidResolver didResolver,
    IOptions<AppViewConfig> opts) : IXrpcQuery<GetProfileQueryRequest, GetProfileQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetProfileQueryResponse>> Handle(
        GetProfileQueryRequest request)
    {
        var userModel = await userService.GetFullUserByDid(request.Did);
        if (userModel is null)
        {
            return XrpcErrorOr<GetProfileQueryResponse>.Fail(
                "ProfileDoesntExist",
                "This profile does not exist.");
        }

        if (userModel.RepoStatus != UserRepoStatus.Active)
        {
            return XrpcErrorOr<GetProfileQueryResponse>.Fail(
                "RepoNotActive",
                "This repo is not active");
        }

        if (userModel.AppViewBlocked)
        {
            return XrpcErrorOr<GetProfileQueryResponse>.Fail(
                "AppViewBlocked",
                "This user has been blocked by the owner of this PinkSea instance.");
        }

        var handle = await didResolver.GetHandleFromDid(request.Did)
            ?? userModel.Handle
            ?? "invalid.handle";

        string? avatarUrl = null;
        if (userModel.Avatar != null)
        {
            avatarUrl = string.Format(
                opts.Value.ImageProxyTemplate,
                userModel.Did,
                userModel.Avatar.BlobCid);
        }

        var links = userModel.Links?
                        .Select(l => new Link { Url = l.Url, Name = l.Name })
                        .ToList() ?? [];

        var profile = new GetProfileQueryResponse
        {
            Did = request.Did,
            Handle = handle,
            Nickname = userModel.Nickname,
            Description = userModel.Description,
            Links = links,
            Avatar = avatarUrl
        };
        
        return XrpcErrorOr<GetProfileQueryResponse>.Ok(profile);
    }
}