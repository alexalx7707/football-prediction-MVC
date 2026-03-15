using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace football_prediction_MVC.Controllers;

[Authorize(Policy = "RequireUserRole")]
public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }
}