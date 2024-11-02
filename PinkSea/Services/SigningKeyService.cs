using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace PinkSea.Services;

/// <summary>
/// The service providing the client signing key.
/// </summary>
public class SigningKeyService(
    ConfigurationService configuration)
{
    /// <summary>
    /// The current elliptic curve used.
    /// </summary>
    private ECDsa? _curve;

    /// <summary>
    /// The current key parameters for the curve.
    /// </summary>
    private ECParameters? _ecParameters;

    /// <summary>
    /// The current security key.
    /// </summary>
    private ECDsaSecurityKey? _securityKey;
    
    /// <summary>
    /// The current key parameters for the curve.
    /// </summary>
    public ECParameters KeyParameters
    {
        get
        {
            if (_ecParameters is null)
                LoadConfiguration();

            return _ecParameters!.Value;
        }
    }

    /// <summary>
    /// The current security key.
    /// </summary>
    public ECDsaSecurityKey SecurityKey
    {
        get
        {
            if (_securityKey is null)
                LoadConfiguration();

            return _securityKey!;
        }
    }

    /// <summary>
    /// The key id of this key.
    /// </summary>
    public string KeyId => configuration.Configuration.KeyId;

    /// <summary>
    /// Loads the configuration.
    /// </summary>
    private void LoadConfiguration()
    {
        _curve = ECDsa.Create(ECCurve.CreateFromFriendlyName("nistp256"));
        _curve.ImportFromPem(configuration.Configuration.ClientPublicKey);
        _curve.ImportFromPem(configuration.Configuration.ClientPrivateKey);

        _ecParameters = _curve.ExportParameters(true);
        _securityKey = new ECDsaSecurityKey(_curve);
    }
}