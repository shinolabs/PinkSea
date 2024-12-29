using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons.Queries;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getIdentity" xrpc query. Returns the identity of the authorized user.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getIdentity")]
public class GetIdentityQueryHandler(IHttpContextAccessor httpContextAccessor,
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IDidResolver didResolver)
    : IXrpcQuery<GetIdentityQueryRequest, GetIdentityQueryResponse>
{
    /// <inheritdoc />
    public async Task<GetIdentityQueryResponse?> Handle(GetIdentityQueryRequest request)
    {
        var state = httpContextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return null!;

        var oauthState = await oAuthStateStorageProvider.GetForStateId(state);
        if (oauthState is null)
            return null!;

        var didDocument = await didResolver.GetDocumentForDid(oauthState.Did);
        return new GetIdentityQueryResponse
        {
            Did = oauthState.Did,
            Handle = didDocument!.GetHandle()!
        };
    }
}