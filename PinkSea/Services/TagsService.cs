using Microsoft.EntityFrameworkCore;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Extensions;

namespace PinkSea.Services;

/// <summary>
/// The service responsible for managing tags.
/// </summary>
public class TagsService(PinkSeaDbContext dbContext)
{
    /// <summary>
    /// Creates a relation between an oekaki model and its tags.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="tags">The tags.</param>
    public async Task CreateRelationBetweenOekakiAndTags(
        OekakiModel model,
        string[] tags)
    {
        var count = 0;
        foreach (var tag in tags)
        {
            var tagModel = await GetOrCreateTag(tag);
            await CreateOekakiTagRelation(model, tagModel);
            
            count++;
            if (count > 10)
                return;
        }
    }

    /// <summary>
    /// Creates or gets a tag given its name.
    /// </summary>
    /// <param name="tag">The tag.</param>
    /// <returns>The tag model.</returns>
    public async Task<TagModel> GetOrCreateTag(string tag)
    {
        var normalized = tag.ToNormalizedTag();
        
        // Find the tag in the database.
        var maybeTag = await dbContext.Tags
            .FirstOrDefaultAsync(t => t.Name == normalized);

        if (maybeTag is not null)
            return maybeTag;
        
        // If not, create it.
        var newTag = new TagModel
        {
            Name = normalized
        };
        
        await dbContext.Tags.AddAsync(newTag);
        await dbContext.SaveChangesAsync();

        return newTag;
    }

    /// <summary>
    /// Creates a relation between an oekaki model and a tag model.
    /// </summary>
    /// <param name="oekakiModel">The oekaki model.</param>
    /// <param name="tagModel">The tag model.</param>
    public async Task CreateOekakiTagRelation(
        OekakiModel oekakiModel,
        TagModel tagModel)
    {
        var relation = new TagOekakiRelationModel
        {
            Oekaki = oekakiModel,
            OekakiId = oekakiModel.Key,

            Tag = tagModel,
            TagId = tagModel.Name
        };

        await dbContext.TagOekakiRelations.AddAsync(relation);
        await dbContext.SaveChangesAsync();
    }
}