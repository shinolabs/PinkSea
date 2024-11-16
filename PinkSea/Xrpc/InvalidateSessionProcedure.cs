using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.invalidateSession" XRPC procedure. Invalidates a given session.
/// </summary>
[Xrpc("com.shinolabs.pinksea.invalidateSession")]
public class InvalidateSessionProcedure(
    IOAuthStateStorageProvider oAuthStateStorageProvider,
    IHttpContextAccessor contextAccessor,
    ILogger<InvalidateSessionProcedure> logger) : IXrpcProcedure<Empty, Empty>
{
    /// <inheritdoc />
    public async Task<Empty?> Handle(Empty request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return new Empty();
        
        logger.LogInformation($"Invalidating session for ID {state}");
        
        await oAuthStateStorageProvider.DeleteForStateId(state);
        return new Empty();
    }
}