using PinkSea.AtProto.Server.Xrpc;
using PinkSea.AtProto.Shared.Xrpc;
using PinkSea.Lexicons.Enums;
using PinkSea.Lexicons.Queries;
using PinkSea.Services;

namespace PinkSea.Xrpc;

/// <summary>
/// The handler for the "com.shinolabs.pinksea.getSearchResults" xrpc query. Gets the search results for a value.
/// </summary>
[Xrpc("com.shinolabs.pinksea.getSearchResults")]
public class GetSearchResultsQueryHandler(
    SearchService searchService,
    FeedBuilder feedBuilder) : IXrpcQuery<GetSearchResultsQueryRequest, GetSearchResultsQueryResponse>
{
    /// <inheritdoc />
    public async Task<XrpcErrorOr<GetSearchResultsQueryResponse>> Handle(GetSearchResultsQueryRequest request)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var since = request.Since ?? DateTimeOffset.UtcNow.AddMinutes(5);

        if (request.Type == SearchType.Posts)
        {
            var posts = await searchService.SearchPosts(request.Query, limit, since);
            var models = await feedBuilder.FromOekakiModelList(posts);

            var result = new GetSearchResultsQueryResponse
            {
                Oekaki = models
            };
            return XrpcErrorOr<GetSearchResultsQueryResponse>.Ok(result);
        }

        if (request.Type == SearchType.Tags)
        {
            var tags = await searchService.SearchTags(request.Query, limit, since);

            var result = new GetSearchResultsQueryResponse
            {
                Tags = tags
            };
            
            return XrpcErrorOr<GetSearchResultsQueryResponse>.Ok(result);
        }

        if (request.Type == SearchType.Profiles)
        {
            var profiles = await searchService.SearchAccounts(request.Query, limit, since);

            var result = new GetSearchResultsQueryResponse
            {
                Profiles = profiles
            };
            
            return XrpcErrorOr<GetSearchResultsQueryResponse>.Ok(result);
        }

        return XrpcErrorOr<GetSearchResultsQueryResponse>.Fail("InvalidSearchType", "Invalid search type.");
    }
}