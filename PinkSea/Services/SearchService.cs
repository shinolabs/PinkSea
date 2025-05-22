using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Lexicons.Objects;
using PinkSea.Models;

namespace PinkSea.Services;

public class SearchService(
    PinkSeaDbContext dbContext,
    IDidResolver didResolver,
    IOptions<AppViewConfig> opts)
{
    public async Task<List<OekakiModel>> SearchPosts(string query, int limit, DateTimeOffset since)
    {
        var list = await dbContext.Oekaki
            .Include(o => o.TagOekakiRelations)
            .Include(o => o.Author)
            .Where(o => !o.Tombstone)
            .Where(o => o.AltText!.ToLower().Contains(query.ToLower()) ||
                        o.TagOekakiRelations!.Any(to => to.TagId.ToLower().Contains(query.ToLower()))) // TODO: Author
            .Distinct()
            .OrderByDescending(o => o.IndexedAt)
            .Where(o => o.IndexedAt < since)
            .Take(limit)
            .ToListAsync();

        return list;
    }

    public async Task<List<TagSearchResult>> SearchTags(string query, int limit, DateTimeOffset since)
    {
        var list = await dbContext.Tags
            .Where(t => t.Name.ToLower().Contains(query.ToLower()))
            .Join(dbContext.TagOekakiRelations, t => t.Name, to => to.TagId, (t, to) => new { t, to })
            .Join(dbContext.Oekaki, c => c.to.OekakiId, o => o.Key, (c, o) => new { c.t, c.to, o })
            .Distinct()
            .GroupBy(c => c.t.Name)
            .Take(limit)
            .Select(c => new
            {
                Tag = c.Key,
                Oekaki = c.First().o,
                Count = c.Count()
            })
            .ToListAsync();
        
        var dids = list.Select(o => o.Oekaki.AuthorDid);
        var map = new ConcurrentDictionary<string, string>();

        await Parallel.ForEachAsync(dids, new ParallelOptions
        {
            MaxDegreeOfParallelism = 5
        }, async (did, _) =>
        {
            map[did] = await didResolver.GetHandleFromDid(did) ?? "invalid.handle";
        });

        var oekakiDtos = list
            .Select(o => new TagSearchResult
            {
                Tag = o.Tag,
                Oekaki = HydratedOekaki.FromOekakiModel(o.Oekaki, map[o.Oekaki.AuthorDid], opts.Value.ImageProxyTemplate),
                Count = o.Count
            })
            .ToList();

        return oekakiDtos;
    }

    public async Task<List<Author>> SearchAccounts(string query, int limit, DateTimeOffset since)
    {
        var list = await dbContext.Users
            .Where(u => u.Handle != null && u.Handle.ToLower().Contains(query))
            .OrderByDescending(u => u.CreatedAt)
            .Where(u => u.CreatedAt < since)
            .Take(limit)
            .ToListAsync();

        return list.Select(u => new Author
        {
            Did = u.Did,
            Handle = u.Handle ?? "invalid.handle"
        }).ToList();
    }
}