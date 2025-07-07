using Microsoft.EntityFrameworkCore;
using PinkSea.AtProto.Helpers;
using PinkSea.AtProto.Models.OAuth;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.AtProto.Shared.Lexicons.AtProto;
using PinkSea.AtProto.Shared.Lexicons.Types;
using PinkSea.AtProto.Xrpc.Client;
using PinkSea.Database;
using PinkSea.Database.Models;
using PinkSea.Lexicons.Records;

namespace PinkSea.Services;

/// <summary>
/// The service responsible for managing users.
/// </summary>
public class UserService(
    PinkSeaDbContext dbContext,
    IDidResolver didResolver,
    IDomainDidResolver domainDidResolver,
    IXrpcClientFactory xrpcClientFactory,
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
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Did == did);
    }
    
    /// <summary>
    /// Gets a full user model by their DID.
    /// </summary>
    /// <param name="did">The DID of the user.</param>
    /// <returns>The user model, or nothing if they don't exist.</returns>
    public async Task<UserModel?> GetFullUserByDid(
        string did)
    {
        return await dbContext.Users
            .Include(u => u.Avatar)
            .Include(u => u.Links)
            .FirstOrDefaultAsync(u => u.Did == did);
    }

    /// <summary>
    /// Gets multiple users at once.
    /// </summary>
    /// <param name="dids">The list of user's DIDs.</param>
    /// <returns>A dictionary mapping from a did to a user model.</returns>
    public async Task<IDictionary<string, UserModel>> GetMultipleUsers(IEnumerable<string> dids)
    {
        var list = await dbContext.Users
            .Where(u => dids.Contains(u.Did))
            .Include(u => u.Avatar)
            .ToListAsync();

        return list.ToDictionary(u => u.Did, u => u);
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

    public async Task UpdateProfile(
        string did,
        Profile profile)
    {
        var user = await GetFullUserByDid(did);
        if (user is null)
            return;

        user.Nickname = profile.Nickname;
        user.Description = profile.Bio;
        await UpdateAvatarForProfile(user, profile.Avatar);
        await UpdateLinksForProfile(user, profile.Links);
        
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Publishes a profile update to the user's repo.
    /// </summary>
    /// <param name="oauthState">The oauth state of the user.</param>
    /// <param name="profile">The changed profile.</param>
    public async Task PublishProfileUpdateToRepo(
        OAuthState oauthState,
        Profile profile)
    {
        using var xrpcClient = await xrpcClientFactory.GetForOAuthState(oauthState);
        if (xrpcClient is null)
            return;
        
        await xrpcClient.Procedure<PutRecordResponse>(
            "com.atproto.repo.putRecord",
            new PutRecordRequest
            {
                Repo = oauthState.Did,
                Collection = "com.shinolabs.pinksea.profile",
                RecordKey = "self",
                Record = profile
            });
    }

    private async Task UpdateLinksForProfile(
        UserModel user,
        IEnumerable<ProfileLink>? links)
    {
        if (user.Links?.Count > 0)
        {
            foreach (var link in user.Links)
            {
                dbContext.Remove(link);
            }
        }

        if (links is null)
            return;

        var newLinks = links.Select(link => new UserLinkModel()
            {
                UserDid = user.Did,
                User = user,
                Url = link.Link,
                Name = link.Name
            })
            .ToList();

        user.Links = newLinks;
    }

    private async Task UpdateAvatarForProfile(
        UserModel user,
        StrongRef? avatarRef)
    {
        if (avatarRef is null)
        {
            user.Avatar = null;
            return;
        }
        
        // Check if the avatar being requested is one this user owns.
        if (!AtLinkHelper.TryParse(avatarRef.Uri, out var atUri))
            return;
        
        // Get the did from the authority
        var authority = atUri.Authority;
        if (!atUri.Authority.StartsWith("did:"))
            authority = await domainDidResolver.GetDidForDomainHandle(authority) ?? authority;

        // Get the avatar oekaki.
        var oekaki = await dbContext.Oekaki
            .Where(o => o.AuthorDid == authority && o.OekakiTid == atUri.RecordKey)
            .FirstOrDefaultAsync();

        user.Avatar = oekaki;
        user.AvatarId = oekaki?.Key;
    }
}