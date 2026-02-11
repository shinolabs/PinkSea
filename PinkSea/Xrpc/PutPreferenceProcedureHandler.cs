using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons;
using PinkSea.Lexicons.Procedures;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// The handler for the "com.shinolabs.pinksea.putPreference" XRPC procedure. Sets a preference for a user.
/// </summary>
[Xrpc("com.shinolabs.pinksea.putPreference")]
public class PutPreferenceProcedureHandler(
    UserService userService,
    PreferencesService preferencesService,
    IHttpContextAccessor contextAccessor,
    IOAuthStateStorageProvider oauthStateStorageProvider)
    : IXrpcProcedure<PutPreferenceProcedureRequest, Empty>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<Empty>> Handle(PutPreferenceProcedureRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return XrpcErrorOr<Empty>.Fail("NoAuthToken", "Missing authorization token.");

        var oauthState = await oauthStateStorageProvider.GetForStateId(state);
        if (oauthState is null)
            return XrpcErrorOr<Empty>.Fail("InvalidToken", "Invalid token.");
        
        var user = await userService.GetUserByDid(oauthState.Did) ?? await userService.Create(oauthState.Did);

        await preferencesService.SetPreferenceForUser(user, request.Key, request.Value);
        if (await preferencesService.PublishPreferencesUpdateToRepo(oauthState))
        {
            return XrpcErrorOr<Empty>.Ok(new Empty());
        }
        
        return XrpcErrorOr<Empty>.Fail("FailedToSave", "Failed to save the preferences to your repository.");
    }
}