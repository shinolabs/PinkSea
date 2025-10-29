using Microsoft.Extensions.Logging;
using NSubstitute;
using PinkSea.AtProto.Models.Authorization;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;
using PinkSea.AtProto.Providers.Storage;
using PinkSea.AtProto.Xrpc.Client;

namespace PinkSea.AtProto.Tests.Xrpc.Client;

[TestFixture]
public class DefaultXrpcClientFactoryTests
{
    private IOAuthStateStorageProvider _storage = null!;
    private IHttpClientFactory _httpFactory = null!;
    private IJwtSigningProvider _jwtSigning = null!;
    private IOAuthClientDataProvider _clientData = null!;
    private ILoggerFactory _loggerFactory = null!;
    private DefaultXrpcClientFactory _sut = null!;

    [SetUp]
    public void Setup()
    {
        _storage = Substitute.For<IOAuthStateStorageProvider>();
        _httpFactory = Substitute.For<IHttpClientFactory>();
        _jwtSigning = Substitute.For<IJwtSigningProvider>();
        _clientData = Substitute.For<IOAuthClientDataProvider>();
        _loggerFactory = Substitute.For<ILoggerFactory>();

        var httpClient = new HttpClient();
        _httpFactory.CreateClient("xrpc-client").Returns(httpClient);

        _sut = new DefaultXrpcClientFactory(_storage, _httpFactory, _jwtSigning, _clientData, _loggerFactory);
    }

    [Test]
    public async Task GetForOAuthStateId_ReturnsNull_WhenStateMissing()
    {
        _storage.GetForStateId("missing").Returns((OAuthState?)null);

        var result = await _sut.GetForOAuthStateId("missing");

        Assert.IsNull(result);
    }

    [Test]
    public async Task GetWithoutAuthentication_ReturnsBasicClient()
    {
        var result = await _sut.GetWithoutAuthentication("https://example.com");
        Assert.IsInstanceOf<BasicXrpcClient>(result);
    }

    [Test]
    public async Task GetForOAuthState_ReturnsSessionClient_ForPdsSession()
    {
        var state = new OAuthState
        {
            AuthorizationType = AuthorizationType.PdsSession,
            Did = "did:plc:test",
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

        var result = await _sut.GetForOAuthState(state);

        Assert.IsInstanceOf<SessionXrpcClient>(result);
    }
}