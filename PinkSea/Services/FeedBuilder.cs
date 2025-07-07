using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Lexicons.Objects;
using PinkSea.Models;

namespace PinkSea.Services;

/// <summary>
/// A feed builder.
/// </summary>
/// <param name="dbContext">The database context.</param>
public class FeedBuilder(
    PinkSeaDbContext dbContext,
    UserService userService,
    IOptions<AppViewConfig> opts)
{
    /// <summary>
    /// The oekaki query.
    /// </summary>
    private IQueryable<OekakiModel> _query = dbContext
        .Oekaki
        .Include(o => o.TagOekakiRelations)
        .Where(o => !o.Tombstone)
        .OrderByDescending(o => o.IndexedAt);

    /// <summary>
    /// Starts a new feed with the given ordering.
    /// </summary>
    /// <param name="expression">The expression to order by.</param>
    /// <param name="descend">Should we descend?</param>
    /// <typeparam name="TKey">The key's type.</typeparam>
    /// <returns>The feed builder.</returns>
    public FeedBuilder StartWithOrdering<TKey>(
        Expression<Func<OekakiModel, TKey>> expression,
        bool descend = false)
    {
        _query = dbContext.Oekaki
            .Include(o => o.TagOekakiRelations)
            .Where(o => !o.Tombstone);
        _query = descend
            ? _query.OrderByDescending(expression)
            : _query.OrderBy(expression);

        return this;
    }
    
    /// <summary>
    /// Adds a where clause to the feed.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The feed builder.</returns>
    public FeedBuilder Where(Expression<Func<OekakiModel, bool>> expression)
    {
        _query = _query.Where(expression);
        return this;
    }

    /// <summary>
    /// Adds filtering by a tag.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <returns>This feed builder.</returns>
    public FeedBuilder WithTag(string tag)
    {
        _query = _query.Where(o =>
            dbContext.TagOekakiRelations
                .Include(r => r.Tag)
                .Any(r => r.OekakiId == o.Key && r.Tag.Name == tag));

        return this;
    }

    /// <summary>
    /// Sets the query to index since some time frame.
    /// </summary>
    /// <param name="since">Since when we should index.</param>
    /// <returns>The feed builder.</returns>
    public FeedBuilder Since(DateTimeOffset since)
    {
        _query = _query.Where(o => o.IndexedAt < since);
        return this;
    }

    /// <summary>
    /// Sets the limit on how many objects to fetch.
    /// </summary>
    /// <param name="count">The count of objects.</param>
    /// <returns>The feed builder.</returns>
    public FeedBuilder Limit(int count)
    {
        _query = _query.Take(count);
        return this;
    }

    /// <summary>
    /// Builds a feed from a list of <see cref="OekakiModel"/>
    /// </summary>
    /// <param name="list">The oekaki models.</param>
    /// <returns>The list of oekaki DTOs.</returns>
    public async Task<List<HydratedOekaki>> FromOekakiModelList(IList<OekakiModel> list)
    {
        var dids = list.Select(o => o.AuthorDid);
        var map = await userService.GetMultipleUsers(dids);

        var oekakiDtos = list
            .Select(o => HydratedOekaki.FromOekakiModel(o, map[o.AuthorDid], opts.Value.ImageProxyTemplate))
            .ToList();

        return oekakiDtos;
    }

    /// <summary>
    /// Gets the feed.
    /// </summary>
    /// <returns>The list of oekaki DTOs.</returns>
    public async Task<List<HydratedOekaki>> GetFeed()
    {
        var list = await _query.ToListAsync();
        return await FromOekakiModelList(list);
    }
}