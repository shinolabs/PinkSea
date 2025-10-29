using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PinkSea.AtProto.Authorization;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Shared.Models.Did;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.AtProto.Xrpc.Client;

namespace PinkSea.AtProto.Tests.Authorization;

[TestFixture]
public class AtProtoAuthorizationServiceTests
{
    private IDidResolver _didResolver = null!;
    private IDomainDidResolver _domainDidResolver = null!;
    private IOAuthStateStorageProvider _stateStorage = null!;
    private IXrpcClientFactory _clientFactory = null!;
    private ILogger<AtProtoAuthorizationService> _logger = null!;
    private IXrpcClient _xrpcClient = null!;
    private AtProtoAuthorizationService _sut = null!;

    [SetUp]
    public void Setup()
    {
        _didResolver = Substitute.For<IDidResolver>();
        _domainDidResolver = Substitute.For<IDomainDidResolver>();
        _stateStorage = Substitute.For<IOAuthStateStorageProvider>();
        _clientFactory = Substitute.For<IXrpcClientFactory>();
        _logger = Substitute.For<ILogger<AtProtoAuthorizationService>>();
        _xrpcClient = Substitute.For<IXrpcClient>();

        _sut = new AtProtoAuthorizationService(
            _didResolver,
            _domainDidResolver,
            _stateStorage,
            _clientFactory,
            _logger);
    }

    [Test]
    public async Task LoginWithPassword_ReturnsError_WhenDIDResolutionFails()
    {
        _domainDidResolver.GetDidForDomainHandle("alice.com").Returns((string?)null);

        var result = await _sut.LoginWithPassword("alice.com", "pw");

        Assert.That(result.IsError, Is.True);
        StringAssert.Contains("Could not resolve the DID", result.Error);
    }

    [Test]
    public async Task LoginWithPassword_ReturnsError_WhenDIDDocumentMissing()
    {
        _domainDidResolver.GetDidForDomainHandle("alice.com").Returns("did:plc:alice");
        _didResolver.GetDocumentForDid("did:plc:alice").Returns((DidDocument?)null);

        var result = await _sut.LoginWithPassword("alice.com", "pw");

        Assert.That(result.IsError, Is.True);
        StringAssert.Contains("Could not fetch the DID document", result.Error);
    }

    [Test]
    public async Task LoginWithPassword_Success_SavesOAuthState()
    {
        var did = "did:plc:alice";
        var pds = "https://pds.example.com";

        var didDoc = new DidDocument
        {
            Id = did,
            Services =
            [
                new DidService
                {
                    Id = "#atproto_pds",
                    Type = "AtprotoPersonalDataServer",
                    ServiceEndpoint = pds
                }
            ]
        };

        var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(expires: DateTime.UtcNow.AddHours(1)));
        var response = new CreateSessionResponse
        {
            AccessToken = jwt,
            RefreshToken = "refresh",
            Did = did,
            Handle = "alice.com",
            Active = true
        };

        _domainDidResolver.GetDidForDomainHandle("alice.com").Returns(did);
        _didResolver.GetDocumentForDid(did).Returns(didDoc);
        _clientFactory.GetWithoutAuthentication(pds).Returns(_xrpcClient);
        _xrpcClient.Procedure<CreateSessionResponse>("com.atproto.server.createSession", Arg.Any<object>())
            .Returns(XrpcErrorOr<CreateSessionResponse>.Ok(response));

        var result = await _sut.LoginWithPassword("alice.com", "pw");

        Assert.That(!result.IsError, Is.True);
        await _stateStorage.Received(1).SetForStateId(Arg.Any<string>(), Arg.Any<OAuthState>());
    }

    [Test]
    public async Task RefreshSession_UpdatesTokens()
    {
        var stateId = "state123";
        var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(expires: DateTime.UtcNow.AddHours(1)));

        var resp = XrpcErrorOr<CreateSessionResponse>.Ok(new CreateSessionResponse
        {
            AccessToken = jwt,
            RefreshToken = "new-refresh",
            Did = "did:plc:alice",
            Handle = "alice.com",
            Active = true
        });

        var state = new OAuthState
        {
            AuthorizationType = AuthorizationType.PdsSession,
            AuthorizationCode = "old",
            RefreshToken = "old",
            Did = "did:plc:alice",
            Issuer = "",
            KeyPair = new DpopKeyPair
            {
                PrivateKey = "",
                PublicKey = ""
            },
            PkceString = "",
            TokenEndpoint = "",
            RevocationEndpoint = "",
            Pds = "https://pds.example.com",
            ExpiresAt = DateTimeOffset.UtcNow
        };

        _clientFactory.GetForOAuthStateId(stateId).Returns(_xrpcClient);
        _xrpcClient.Procedure<CreateSessionResponse>("com.atproto.server.refreshSession", null).Returns(resp);
        _stateStorage.GetForStateId(stateId).Returns(state);

        var ok = await _sut.RefreshSession(stateId);

        Assert.That(ok, Is.True);
        await _stateStorage.Received(1)
            .SetForStateId(stateId, Arg.Is<OAuthState>(s => s.RefreshToken == "new-refresh"));
    }

    [Test]
    public async Task InvalidateSession_DeletesStateAfterRequest()
    {
        var stateId = "abc";
        _clientFactory.GetForOAuthStateId(stateId).Returns(_xrpcClient);
        _xrpcClient.Procedure<object>("com.atproto.server.deleteSession", null)
            .Returns(XrpcErrorOr<object>.Ok(new()));

        await _sut.InvalidateSession(stateId);

        await _stateStorage.Received(1).DeleteForStateId(stateId);
    }
}