using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Lexicons.Queries;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getHandleFromDid" xrpc query. Returns the handle for a DID.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getHandleFromDid")]
public class GetHandleFromDidQueryHandler(IDidResolver didResolver)
    : IXrpcQuery<GetHandleFromDidQueryRequest, GetHandleFromDidQueryResponse>
{
    /// <inheritdoc />
    public async Task<GetHandleFromDidQueryResponse?> Handle(GetHandleFromDidQueryRequest request)
    {
        var handle = await didResolver.GetHandleFromDid(request.Did);
        
        return new GetHandleFromDidQueryResponse()
        {
            Handle = handle ?? request.Did
        };
    }
}