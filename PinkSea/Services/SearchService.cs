using Microsoft.EntityFrameworkCore;
using PinkSea.Database;
using PinkSea.Database.Models;

namespace PinkSea.Services;

public class SearchService(
    PinkSeaDbContext dbContext)
{
    public async Task<List<OekakiModel>> SearchPosts(string query, int limit, DateTimeOffset since)
    {
        var list = await dbContext.Oekaki
            .Include(o => o.TagOekakiRelations)
            .Include(o => o.Author)
            .Where(o => !o.Tombstone)
            .Where(o => o.AltText!.ToLower().Contains(query.ToLower()) ||
                        o.TagOekakiRelations!.Any(to => to.TagId.ToLower().Contains(query.ToLower()))) // TODO: Author
            .OrderByDescending(o => o.IndexedAt)
            .Where(o => o.IndexedAt < since)
            .Take(limit)
            .ToListAsync();

        return list;
    }
}