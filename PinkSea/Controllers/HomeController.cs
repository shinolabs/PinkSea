using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PinkSea.AtProto.OAuth;
using PinkSea.AtProto.Resolvers.Did;
using PinkSea.AtProto.Resolvers.Domain;
using PinkSea.Models;

namespace PinkSea.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDomainDidResolver _resolver;
    private readonly IDidResolver _didResolver;
    private readonly IAtProtoOAuthClient _atProtoOAuthClient;
    
    public HomeController(
        ILogger<HomeController> logger,
        IDomainDidResolver resolver,
        IDidResolver didResolver,
        IAtProtoOAuthClient atProtoOAuthClient)
    {
        _logger = logger;
        _resolver = resolver;
        _didResolver = didResolver;
        _atProtoOAuthClient = atProtoOAuthClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Test([FromQuery] string handle)
    {
        var authorizationServer = await _atProtoOAuthClient.GetOAuthRequestUriForHandle(handle, "");
        return Redirect(authorizationServer);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}