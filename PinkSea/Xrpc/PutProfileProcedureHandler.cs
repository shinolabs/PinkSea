using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons;
using PinkSea.Lexicons.Procedures;
using PinkSea.Services;
using PinkSea.Validators;

namespace PinkSea.Xrpc;

/// <summary>
/// The handler for the "com.shinolabs.pinksea.putProfile" XRPC procedure. Updates a user's profile.
/// </summary>
[Xrpc("com.shinolabs.pinksea.putProfile")]
public class PutProfileProcedureHandler(IHttpContextAccessor contextAccessor, IOAuthStateStorageProvider oauthStateStorageProvider, UserService userService)
    : IXrpcProcedure<PutProfileProcedureRequest, Empty>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<Empty>> Handle(PutProfileProcedureRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return XrpcErrorOr<Empty>.Fail("NoAuthToken", "Missing authorization token.");

        var oauthState = await oauthStateStorageProvider.GetForStateId(state);
        if (oauthState is null)
            return XrpcErrorOr<Empty>.Fail("InvalidToken", "Invalid token.");

        var validator = new ProfileValidator();

        if (!validator.Validate(request.Profile))
            return XrpcErrorOr<Empty>.Fail("InvalidRecord", "Invalid profile record.");

        await userService.UpdateProfile(oauthState.Did, request.Profile);
        await userService.PublishProfileUpdateToRepo(oauthState, request.Profile);
        
        return XrpcErrorOr<Empty>.Ok(new Empty());
    }
}