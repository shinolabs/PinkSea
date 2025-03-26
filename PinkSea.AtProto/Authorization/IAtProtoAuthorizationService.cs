using PinkSea.AtProto.Shared.Models;

namespace PinkSea.AtProto.Authorization;

/// <summary>
/// The service responsible for non-OAuth2 authorization with AtProto.
/// </summary>
public interface IAtProtoAuthorizationService
{
    /// <summary>
    /// Logs in a user using a password,
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="password">The password.</param>
    /// <returns>The state code for the user.</returns>
    public Task<ErrorOr<string>> LoginWithPassword(string handle, string password);

    /// <summary>
    /// Refreshes the session.
    /// </summary>
    /// <param name="stateId"></param>
    /// <returns></returns>
    public Task<bool> RefreshSession(string stateId);
}