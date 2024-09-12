using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using finTrack.Models;
using finTrack.Interfaces;

namespace finTrack.Controllers;

public class HomeController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly IAcountRepository _acountRepo;

    public HomeController(ITokenService tokenservice, IAcountRepository acountRepo)
    {
        _tokenService = tokenservice;
        _acountRepo = acountRepo;
    }

    public async Task<IActionResult> IndexAsync()
    {
        var UserID = HttpContext.Items["UserID"]?.ToString();
        var user = await _acountRepo.GetUserByGuidAsync(UserID);
        if (user != null) return RedirectToAction("Content", "Home");
        return View();
    }

    public IActionResult PrivacyAsync()
    {
        return View();
    }

    public async Task<IActionResult> RegisterAsync()
    {
        var UserID = HttpContext.Items["UserID"]?.ToString();
        var user = await _acountRepo.GetUserByGuidAsync(UserID);
        if (user != null) return RedirectToAction("Content", "Home");

        return View();
    }

    public async Task<IActionResult> ContentAsync()
    {
        if (!HttpContext.Items.ContainsKey("UserID"))return RedirectToAction("Index", "Home");
        var UserID = HttpContext.Items["UserID"]?.ToString();
        if (string.IsNullOrEmpty(UserID)) return RedirectToAction("Index", "Home");
        var user = await _acountRepo.GetUserByGuidAsync(UserID);
        if (user == null) return RedirectToAction("Index", "Home");

        ViewBag.Balance = user.Balance.ToString("F2");
        ViewBag.UserName = user.UserName;
        ViewBag.Name = $@"{user.FirstName} {user.LastName}";
        return View();
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
