using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.UI.Models;
using UnganaConnect.UI.Services;

namespace UnganaConnect.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AuthService _authService;

    public HomeController(ILogger<HomeController> logger, AuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public IActionResult Index()
    {
        if (_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return RedirectToAction("Login", "Auth");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
