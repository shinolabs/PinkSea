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

    /// <summary>
    /// Generates the DPoP header.
    /// </summary>
    /// <param name="signingData">The DPoP signing data.</param>
    /// <returns>The DPoP header value.</returns>
    string GenerateDpopHeader(DpopSigningData signingData);
}