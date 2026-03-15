using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace football_prediction_MVC.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Dashboard()
    {
        return View();
    }
}