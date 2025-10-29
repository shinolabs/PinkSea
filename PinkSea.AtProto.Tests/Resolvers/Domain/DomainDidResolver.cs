using System.Net;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using PinkSea.AtProto.Resolvers.Domain;

namespace PinkSea.AtProto.Tests.Resolvers.Domain;

[TestFixture]
public class DomainDidResolverTests
{
    private IDnsQuery _dnsQuery = null!;
    private IHttpClientFactory _httpFactory = null!;
    private IMemoryCache _memoryCache = null!;
    private DomainDidResolver _resolver = null!;

    [SetUp]
    public void SetUp()
    {
        _dnsQuery = Substitute.For<IDnsQuery>();
        _httpFactory = Substitute.For<IHttpClientFactory>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _resolver = new DomainDidResolver(_dnsQuery, _httpFactory, _memoryCache);
    }

    [Test]
    public async Task GetDidForDomainHandle_Uses_Cache()
    {
        const string handle = "example.com";
        const string did = "did:plc:1234abcd";

        var queryResponse = Substitute.For<IDnsQueryResponse>();
        var info = new ResourceRecordInfo(
            "_atproto." + handle + ".",
            ResourceRecordType.TXT,
            QueryClass.IN,
            60,
            0);

        var txtRecord = new TxtRecord(info,
            [$"did={did}"], 
            [$"did={did}"]);
        
        queryResponse.Answers.Returns([txtRecord]);
        _dnsQuery.QueryAsync(Arg.Any<string>(), QueryType.TXT).Returns(Task.FromResult(queryResponse));

        var result1 = await _resolver.GetDidForDomainHandle(handle);
        var result2 = await _resolver.GetDidForDomainHandle(handle);
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.EqualTo(did));
            Assert.That(result2, Is.EqualTo(did));
        });
        await _dnsQuery.Received(1).QueryAsync(Arg.Any<string>(), QueryType.TXT);
    }

    [Test]
    public async Task TryResolveDidThroughDnsTxt_Returns_Null_When_No_Record()
    {
        var queryResponse = Substitute.For<IDnsQueryResponse>();
        queryResponse.Answers.Returns(Array.Empty<DnsClient.Protocol.DnsResourceRecord>());
        _dnsQuery.QueryAsync(Arg.Any<string>(), QueryType.TXT).Returns(Task.FromResult(queryResponse));

        var result = await InvokePrivate<string?>(_resolver, "TryResolveDidThroughDnsTxt", "example.com");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task TryResolveDidThroughDnsTxt_Returns_Did_When_Record_Present()
    {
        const string did = "did:plc:abcd1234";
        var info = new ResourceRecordInfo(
            "_atproto.example.com.",
            ResourceRecordType.TXT,
            QueryClass.IN,
            60,
            0);

        var txtRecord = new TxtRecord(
            info,
            [$"did={did}"],
            [$"did={did}"]);

        var queryResponse = Substitute.For<IDnsQueryResponse>();
        queryResponse.Answers.Returns([txtRecord]);
        _dnsQuery.QueryAsync(Arg.Any<string>(), QueryType.TXT).Returns(Task.FromResult(queryResponse));

        var result = await InvokePrivate<string?>(_resolver, "TryResolveDidThroughDnsTxt", "example.com");

        Assert.That(result, Is.EqualTo(did));
    }

    [Test]
    public async Task TryResolveDidThroughWellKnown_Returns_Null_On_NonSuccess()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NotFound, "");
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com")
        };
        _httpFactory.CreateClient("domain-did-resolver").Returns(client);

        var result = await InvokePrivate<string?>(_resolver, "TryResolveDidThroughWellKnown", "example.com");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task TryResolveDidThroughWellKnown_Returns_Trimmed_Content_On_Success()
    {
        const string did = "did:plc:xyz987";
        var handler = new FakeHttpHandler(HttpStatusCode.OK, $"  {did}  ");
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com")
        };
        _httpFactory.CreateClient("domain-did-resolver").Returns(client);

        var result = await InvokePrivate<string?>(_resolver, "TryResolveDidThroughWellKnown", "example.com");

        Assert.That(result, Is.EqualTo(did));
    }

    [Test]
    public async Task GetDidForDomainHandle_FallsBack_To_WellKnown_When_Dns_Fails()
    {
        const string handle = "fallback.test";
        const string did = "did:plc:fallback";

        var queryResponse = Substitute.For<IDnsQueryResponse>();
        queryResponse.Answers.Returns(Array.Empty<DnsClient.Protocol.DnsResourceRecord>());
        _dnsQuery.QueryAsync(Arg.Any<string>(), QueryType.TXT).Returns(Task.FromResult(queryResponse));

        var handler = new FakeHttpHandler(HttpStatusCode.OK, did);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://" + handle)
        };
        _httpFactory.CreateClient("domain-did-resolver").Returns(client);

        var result = await _resolver.GetDidForDomainHandle(handle);
        Assert.That(result, Is.EqualTo(did));
    }

    private static async Task<T?> InvokePrivate<T>(object obj, string methodName, params object[] args)
    {
        var method = obj.GetType().GetMethod(methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var result = method.Invoke(obj, args);
        if (result is Task<T> taskT)
            return await taskT;
        if (result is Task task)
        {
            await task;
            return default;
        }
        return (T?)result;
    }

    private class FakeHttpHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public FakeHttpHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content)
            };
            return Task.FromResult(resp);
        }
    }
}