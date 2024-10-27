namespace PinkSea.AtProto.Providers.OAuth;

/// <summary>
/// A JWT signing provider.
/// </summary>
public interface IJwtSigningProvider
{
    /// <summary>
    /// Generates a client assertion.
    /// </summary>
    /// <param name="signingData">The data used for signing.</param>
    /// <returns>The client assertion.</returns>
    string GenerateClientAssertion(JwtSigningData signingData);
}