using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using PinkSea.AtProto.Helpers;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Shared.Models;
using PinkSea.AtProto.Xrpc.Client;

namespace PinkSea.AtProto.Authorization;

/// <summary>
/// The AT Protocol authorization service.
/// </summary>
public class AtProtoAuthorizationService(
    IDidResolver didResolver,
    IDomainDidResolver domainDidResolver,
    IOAuthStateStorageProvider oauthStateStorageProvider,
    IXrpcClientFactory xrpcClientFactory,
    ILogger<AtProtoAuthorizationService> logger) : IAtProtoAuthorizationService
{
    /// <inheritdoc />
    public async Task<ErrorOr<string>> LoginWithPassword(string handle, string password)
    {
        var identifier = handle.StartsWith("did")
            ? handle
            : await domainDidResolver.GetDidForDomainHandle(handle);

        if (identifier is null)
            return ErrorOr<string>.Fail($"Could not resolve the DID for {handle}.");

        var didDocument = await didResolver.GetDocumentForDid(identifier);
        if (didDocument is null)
            return ErrorOr<string>.Fail($"Could not fetch the DID document for {identifier}.");

        var pds = didDocument.GetPds()!;
        using var xrpcClient = await xrpcClientFactory.GetWithoutAuthentication(pds);
        var resp = await xrpcClient.Procedure<CreateSessionResponse>(
            "com.atproto.server.createSession",
            new
            {
                identifier,
                password
            });

        if (!resp.IsSuccess)
        {
            logger.LogError($"Failed login for {handle} with reason {resp.Error}");
            return ErrorOr<string>.Fail(resp.Error!.Message ?? resp.Error.Error);
        }

        var tokenResponse = resp.Value!;
        if (!tokenResponse.Active)
            return ErrorOr<string>.Fail($"The password token is not active.");

        var jwt = new JwtSecurityTokenHandler();
        var jwtToken = jwt.ReadJwtToken(tokenResponse.AccessToken);

        var expiry = new DateTimeOffset(jwtToken.ValidTo)
            .UtcDateTime;
        
        var oauthState = new OAuthState
        {
            AuthorizationType = AuthorizationType.PdsSession,
            AuthorizationCode = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            Did = identifier,
            Pds = pds,
            PkceString = "",
            Issuer = "",
            TokenEndpoint = "",
            RevocationEndpoint = "",
            KeyPair = new DpopKeyPair()
            {
                PrivateKey = "",
                PublicKey = ""
            },
            ExpiresAt = expiry
        };

        var stateId = StateHelper.GenerateRandomState();
        await oauthStateStorageProvider.SetForStateId(stateId, oauthState);
        
        return ErrorOr<string>.Ok(stateId);
    }

    /// <inheritdoc />
    public async Task<bool> RefreshSession(string stateId)
    {
        using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateId);
        if (xrpcClient is null)
            return false;

        var resp = await xrpcClient.Procedure<CreateSessionResponse>("com.atproto.server.refreshSession");
        if (!resp.IsSuccess)
            return false;

        var jwt = new JwtSecurityTokenHandler();
        var jwtToken = jwt.ReadJwtToken(resp.Value!.AccessToken);

        var expiry = new DateTimeOffset(jwtToken.ValidTo)
            .UtcDateTime;
        
        var oauthState = await oauthStateStorageProvider.GetForStateId(stateId);
        oauthState!.AuthorizationCode = resp.Value.AccessToken;
        oauthState.RefreshToken = resp.Value.RefreshToken;
        oauthState.ExpiresAt = expiry;

        await oauthStateStorageProvider.SetForStateId(stateId, oauthState);

        return true;
    }

    /// <inheritdoc />
    public async Task InvalidateSession(string stateId)
    {
        try
        {
            using var xrpcClient = await xrpcClientFactory.GetForOAuthStateId(stateId);
            if (xrpcClient is null)
                return;

            await xrpcClient.Procedure<object>("com.atproto.server.deleteSession");
        }
        finally
        {
            await oauthStateStorageProvider.DeleteForStateId(stateId);
        }
    }
}