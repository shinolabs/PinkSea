using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.Database;
using PinkSea.Database.Models;

namespace PinkSea.Services;

/// <summary>
/// The service responsible for managing users.
/// </summary>
public class UserService(
    PinkSeaDbContext dbContext,
    IDidResolver didResolver,
    ILogger<UserService> logger)
{
    /// <summary>
    /// Checks whether a given user exists.
    /// </summary>
    /// <param name="did">The user's DID.</param>
    /// <returns>Whether they exist.</returns>
    public async Task<bool> UserExists(
        string did)
    {
        return await dbContext.Users
            .AnyAsync(u => u.Did == did);
    }

    /// <summary>
    /// Gets a user by their DID.
    /// </summary>
    /// <param name="did">The DID of the user.</param>
    /// <returns>The user model, or nothing if they don't exist.</returns>
    public async Task<UserModel?> GetUserByDid(
        string did)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Did == did);
    }

    /// <summary>
    /// Gets all of the users.
    /// </summary>
    /// <returns>All of the users.</returns>
    public async Task<List<UserModel>> GetAllUsers()
    {
        return await dbContext.Users
            .ToListAsync();
    }
    
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="did">The DID of the user.</param>
    /// <returns>The user model.</returns>
    public async Task<UserModel> Create(
        string did)
    {
        var handle = await didResolver.GetHandleFromDid(did);
        logger.LogInformation("Creating a new user with DID {Did} and handle {Handle}",
            did, handle);
        
        var author = new UserModel
        {
            Did = did,
            Handle = handle,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await dbContext.Users.AddAsync(author);
        await dbContext.SaveChangesAsync();

        return author;
    }

    /// <summary>
    /// Updates the repo status for a user.
    /// </summary>
    /// <param name="did">The DID of the user.</param>
    /// <param name="repoStatus">The status of the repository.</param>
    public async Task UpdateRepoStatus(
        string did,
        UserRepoStatus repoStatus)
    {
        logger.LogInformation("Updating the repo status of {Did}. New repo status is {Status}",
            did, repoStatus);

        await dbContext.Users
            .Where(u => u.Did == did)
            .ExecuteUpdateAsync(sp => sp.SetProperty(u => u.RepoStatus, repoStatus));
    }
    
    /// <summary>
    /// Updates the handle for a user.
    /// </summary>
    /// <param name="did">The DID of the user.</param>
    /// <param name="newHandle">The new handle of said user.</param>
    public async Task UpdateHandle(
        string did,
        string newHandle)
    {
        logger.LogInformation("Updating the handle of {Did}. New handle is @{Handle}",
            did, newHandle);
        
        await dbContext.Users
            .Where(u => u.Did == did)
            .ExecuteUpdateAsync(sp => sp.SetProperty(u => u.Handle, newHandle));
    }
}