using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Shared.Xrpc;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.atproto.identity.resolveHandle" XRPC query.
/// Resolves an atproto handle (hostname) to a DID.
/// </summary>
[Xrpc("com.atproto.identity.resolveHandle")]
public class ResolveHandleQueryHandler(IDomainDidResolver domainDidResolver)
    : IXrpcQuery<ResolveHandleRequest, ResolveHandleResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<ResolveHandleResponse>> Handle(ResolveHandleRequest request)
    {
        string? did;
        try
        {
            did = await domainDidResolver.GetDidForDomainHandle(request.Handle);
        }
        catch
        {
            return XrpcErrorOr<ResolveHandleResponse>.Fail("InvalidRequest", "Unable to resolve handle");
        }
        
        if (string.IsNullOrEmpty(did))
            return XrpcErrorOr<ResolveHandleResponse>.Fail("InvalidRequest", "Unable to resolve handle");
            
        return XrpcErrorOr<ResolveHandleResponse>.Ok(new ResolveHandleResponse
        {
            Did = did
        });
    }
}