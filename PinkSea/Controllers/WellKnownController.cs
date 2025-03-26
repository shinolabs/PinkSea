using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PinkSea.AtProto.Shared.Models.Did;
using PinkSea.Models;

namespace PinkSea.Controllers;

/// <summary>
/// Controller for the ".well-known/" routes.
/// </summary>
[Route(".well-known")]
public class WellKnownController(
    IOptions<AppViewConfig> options) : ControllerBase
{
    /// <summary>
    /// Returns the did:web for this PinkSea AppView. 
    /// </summary>
    /// <returns>The DID.</returns>
    [Route("did.json")]
    public DidDocument Did()
    {
        var host = new Uri(options.Value.AppUrl)
            .Host;
        
        return new DidDocument()
        {
            Context = 
            [
                "https://www.w3.org/ns/did/v1"
            ],
            Id = $"did:web:{host}",
            Services =
            [
                new DidService
                {
                    Id = "#pinksea_appview",
                    Type = "PinkSeaApiService",
                    ServiceEndpoint = $"{options.Value.AppUrl}"
                }
            ]
        };
    }
}