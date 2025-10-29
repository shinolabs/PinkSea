using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NUnit.Framework;
using PinkSea.AtProto.Http;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Providers.OAuth;

namespace PinkSea.AtProto.Tests.Http
{
    [TestFixture]
    public class DpopHttpClientTests
    {
        private IJwtSigningProvider _jwtSigningProvider;
        private ILogger _logger;
        private OAuthClientData _clientData;
        private DpopKeyPair _keyPair;

        [SetUp]
        public void Setup()
        {
            _jwtSigningProvider = Substitute.For<IJwtSigningProvider>();
            _logger = Substitute.For<ILogger>();
            _clientData = new OAuthClientData
            {
                ClientId = "client-xyz",
                RedirectUri = "https://redirect.test",
                Key = new JwtKey
                {
                    KeyId = "kid123",
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey1234567890")),
                        SecurityAlgorithms.HmacSha256)
                }
            };
            _keyPair = new DpopKeyPair
            {
                PublicKey = "PUBLIC_KEY_PEM",
                PrivateKey = "PRIVATE_KEY_PEM"
            };
        }

        private DpopHttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> sendFunc)
        {
            var handler = Substitute.ForPartsOf<HttpMessageHandler>();
            handler
                .When(x => x.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()))
                .DoNotCallBase();
            handler
                .SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var req = ci.Arg<HttpRequestMessage>();
                    return Task.FromResult(sendFunc(req));
                });

            var client = new HttpClient(handler);
            return new DpopHttpClient(client, _jwtSigningProvider, _clientData, _logger);
        }

        [Test]
        public void SetAuthorizationCode_ShouldComputeSHA256AndStoreIt()
        {
            var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK));

            client.SetAuthorizationCode("abc123");

            var field = typeof(DpopHttpClient).GetField("_authorizationCode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var actual = (string)field.GetValue(client)!;

            var expected = Base64UrlEncoder.Encode(SHA256.HashData(Encoding.ASCII.GetBytes("abc123")));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task Send_ShouldIncludeDpopHeader_AndAuthorization_WhenSet()
        {
            // Arrange
            var expectedJwt = "dpop.jwt.header";
            _jwtSigningProvider.GenerateDpopHeader(Arg.Any<DpopSigningData>()).Returns(expectedJwt);
            var capturedRequest = (HttpRequestMessage?)null;

            var httpClient = CreateClient(req =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            httpClient.SetAuthorizationCode("access_token");

            // Act
            var result = await httpClient.Send("https://example.com/api/test", HttpMethod.Post, _keyPair);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(capturedRequest, Is.Not.Null);
            Assert.That(capturedRequest!.Headers.Contains("DPoP"), Is.True);
            Assert.That(capturedRequest.Headers.GetValues("DPoP").First(), Is.EqualTo(expectedJwt));
            Assert.That(capturedRequest.Headers.Authorization!.Scheme, Is.EqualTo("DPoP"));
            Assert.That(capturedRequest.Headers.Authorization!.Parameter, Is.EqualTo("access_token"));
        }

        [Test]
        public async Task Send_ShouldRetry_WhenUnauthorizedAndNonceHeaderPresent()
        {
            // Arrange
            _jwtSigningProvider.GenerateDpopHeader(Arg.Any<DpopSigningData>()).Returns("header1", "header2");
            var callCount = 0;

            var client = CreateClient(req =>
            {
                callCount++;
                if (callCount == 1)
                {
                    var r = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    r.Headers.Add("DPoP-Nonce", "nonce123");
                    return r;
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            // Act
            var response = await client.Send("https://retry.test", HttpMethod.Get, _keyPair);

            // Assert
            Assert.That(callCount, Is.EqualTo(2));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            _jwtSigningProvider.Received(2).GenerateDpopHeader(Arg.Any<DpopSigningData>());
        }

        [Test]
        public async Task Send_ShouldNotRetry_WhenUnauthorized_WithoutNonce()
        {
            // Arrange
            var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            _jwtSigningProvider.GenerateDpopHeader(Arg.Any<DpopSigningData>()).Returns("jwt-header");
            var client = CreateClient(_ => msg);

            // Act
            var response = await client.Send("https://no-nonce.test", HttpMethod.Get, _keyPair);

            // Assert
            Assert.That(response, Is.EqualTo(msg));
            await _logger.Received().LogWarning(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public async Task Send_ShouldReturnResponse_WhenStatusIsOK()
        {
            // Arrange
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            _jwtSigningProvider.GenerateDpopHeader(Arg.Any<DpopSigningData>()).Returns("jwt");
            var client = CreateClient(_ => resp);

            // Act
            var result = await client.Send("https://ok.test", HttpMethod.Get, _keyPair);

            // Assert
            Assert.That(result, Is.EqualTo(resp));
            await _logger.DidNotReceive().LogWarning(Arg.Any<string>(), Arg.Any<object>());
        }

        [Test]
        public async Task Post_ShouldSendJsonBody()
        {
            // Arrange
            _jwtSigningProvider.GenerateDpopHeader(Arg.Any<DpopSigningData>()).Returns("jwt");
            var captured = (HttpRequestMessage?)null;

            var client = CreateClient(req =>
            {
                captured = req;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var payload = new { Name = "John", Age = 30 };

            // Act
            await client.Post("https://example.com/users", payload, _keyPair);

            // Assert
            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(captured.Content, Is.Not.Null);

            var json = await captured.Content!.ReadAsStringAsync();
