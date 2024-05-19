using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

public class SignUpController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        throw new NotImplementedException();
    }
}