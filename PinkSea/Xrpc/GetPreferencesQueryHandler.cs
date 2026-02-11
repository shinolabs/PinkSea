using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Extensions;
using PinkSea.Lexicons.Objects;
using PinkSea.Lexicons.Queries;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// Handler for the "com.shinolabs.pinksea.getPreferences" xrpc call. Retrieves all the preferences for a given user.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getPreferences")]
public class GetPreferencesQueryHandler(
    UserService userService,
    PreferencesService preferencesService,
    IHttpContextAccessor contextAccessor,
    IOAuthStateStorageProvider oauthStateStorageProvider)
    : IXrpcQuery<GetPreferencesQueryRequest, GetPreferencesQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetPreferencesQueryResponse>> Handle(GetPreferencesQueryRequest request)
    {
        var state = contextAccessor.HttpContext?.GetStateToken();
        if (state is null)
            return XrpcErrorOr<GetPreferencesQueryResponse>.Fail("NoAuthToken", "Missing authorization token.");

        var oauthState = await oauthStateStorageProvider.GetForStateId(state);
        if (oauthState is null)
            return XrpcErrorOr<GetPreferencesQueryResponse>.Fail("InvalidToken", "Invalid token.");

        var user = await userService.GetUserByDid(oauthState.Did);
        if (user is null)
        {
            // If the user is null, we should probably make one for them. We can also return a blank response, as we have no data.
            await userService.Create(oauthState.Did);
            return XrpcErrorOr<GetPreferencesQueryResponse>.Ok(new GetPreferencesQueryResponse
            {
                Preferences = []
            });
        }

        var preferences = await preferencesService.GetAllPreferencesForUser(user);
        return XrpcErrorOr<GetPreferencesQueryResponse>.Ok(new GetPreferencesQueryResponse
        {
            Preferences = preferences.Select(p => new Preference
            {
                Key = p.Key,
                Value = p.Value
            }).ToList()
        });
    }
}