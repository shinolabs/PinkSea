using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Shared.Models.Did;

namespace PinkSea.AtProto.Tests.Resolvers.Did;

[TestFixture]
public class DidResolverTests
{
    private IHttpClientFactory _clientFactory = null!;
    private IMemoryCache _memoryCache = null!;
    private ILogger<DidResolver> _logger = null!;
    private DidResolver _resolver = null!;

    [SetUp]
    public void SetUp()
    {
        _clientFactory = Substitute.For<IHttpClientFactory>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Substitute.For<ILogger<DidResolver>>();

        _resolver = new DidResolver(_clientFactory, _memoryCache, _logger);
    }

    private static DidDocument CreateSampleDocument() => new()
    {
        Id = "did:plc:xtzdbt3kmdb6ef3wumhkhktd",
        Services = new[]
        {
            new DidService
            {
                Id = "#atproto_pds",
                Type = "AtprotoPersonalDataServer",
                ServiceEndpoint = "https://porcini.us-east.host.bsky.network"
            }
        },
        AlsoKnownAs = new[] { "at://prefetcher.miku.place" }
    };

    [Test]
    public async Task GetDocumentForDid_Caches_Resolved_Result()
    {
        var handler = new FakeHttpHandler(CreateSampleDocument());
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://plc.directory")
        };

        _clientFactory.CreateClient("did-resolver").Returns(client);

        var result1 = await _resolver.GetDocumentForDid("did:plc:xtzdbt3kmdb6ef3wumhkhktd");
        var result2 = await _resolver.GetDocumentForDid("did:plc:xtzdbt3kmdb6ef3wumhkhktd");

        Assert.That(result1, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result1!.Id, Is.EqualTo("did:plc:xtzdbt3kmdb6ef3wumhkhktd"));
            Assert.That(result2, Is.SameAs(result1), "Should return cached result");
        });
    }

    [Test]
    public async Task GetHandleFromDid_Returns_Handle_From_DidDocument()
    {
        var handler = new FakeHttpHandler(CreateSampleDocument());
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://plc.directory")
        };

        _clientFactory.CreateClient("did-resolver").Returns(client);

        var handle = await _resolver.GetHandleFromDid("did:plc:xtzdbt3kmdb6ef3wumhkhktd");

        Assert.That(handle, Is.EqualTo("prefetcher.miku.place"));
    }

    [Test]
    public async Task ResolveDidViaWeb_Returns_Null_On_HttpException()
    {
        var handler = new ThrowingHttpHandler();
        var client = new HttpClient(handler);
        _clientFactory.CreateClient().Returns(client);

        var doc = await InvokePrivateMethod<DidDocument?>(
            _resolver, "ResolveDidViaWeb", "example.com");

        Assert.That(doc, Is.Null);
    }

    [Test]
    public async Task ResolveDidViaPlcDirectory_Returns_Null_On_HttpException()
    {
        var handler = new ThrowingHttpHandler();
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://directory.fake")
        };
        _clientFactory.CreateClient("did-resolver").Returns(client);

        var doc = await InvokePrivateMethod<DidDocument?>(
            _resolver, "ResolveDidViaPlcDirectory", "did:plc:bad");

        Assert.That(doc, Is.Null);
    }

    [Test]
    public async Task ResolveDid_ReturnsNull_For_Invalid_Uri_Format()
    {
        var result = await InvokePrivateMethod<DidDocument?>(
            _resolver, "ResolveDid", "not-a-did");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ResolveDid_ReturnsNull_For_NonDid_Scheme()
    {
        var result = await InvokePrivateMethod<DidDocument?>(
            _resolver, "ResolveDid", "http://example.com");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ResolveDid_ReturnsNull_For_Unsupported_Method()
    {
        var result = await InvokePrivateMethod<DidDocument?>(
            _resolver, "ResolveDid", "did:example:foo");

        Assert.That(result, Is.Null);
    }
    
    [Test]
    public void ResolveDid_Throws_For_Empty_Domain_In_DidWeb()
    {
        Assert.ThrowsAsync<UriFormatException>(async () =>
            await InvokePrivateMethod<DidDocument?>(
                _resolver, "ResolveDid", "did:web:"));
    }

    private static async Task<T> InvokePrivateMethod<T>(object obj, string methodName, params object[] args)
    {
        var method = obj.GetType().GetMethod(methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        var result = method.Invoke(obj, args);

        switch (result)
        {
            case Task<T> taskT:
                return await taskT;
            case Task task:
                await task;
                return default!;
            default:
                return (T)result!;
        }
    }

    private class FakeHttpHandler : HttpMessageHandler
    {
        private readonly DidDocument _doc;

        public FakeHttpHandler(DidDocument doc) => _doc = doc;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(_doc)
            };
            return Task.FromResult(response);
        }
    }

    private class ThrowingHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => throw new HttpRequestException("Simulated failure");
    }
}