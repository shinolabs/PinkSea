using System.Security.Cryptography;

namespace PinkSea.AtProto.Helpers;

/// <summary>
/// The helper for OAuth states.
/// </summary>
public static class StateHelper
{
    /// <summary>
    /// Generates a random state string.
    /// </summary>
    /// <returns>The state string.</returns>
    public static string GenerateRandomState()
    {
        return RandomNumberGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-", 64);
    }
}