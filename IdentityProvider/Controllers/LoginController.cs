using IdentityProvider.Constants;
using IdentityProvider.Contracts;
using IdentityProvider.Controllers.Abstracts;
using IdentityProvider.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[Route("connect/[controller]")]
public class LoginController : OAuthControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(new LoginViewModel
        {
            LoginHint = ""
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Consumes(MimeTypeConstants.FormUrlEncoded)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        PostLoginRequest request,
        CancellationToken cancellationToken)
    {
        // handle the login
        throw new NotImplementedException();
    }
}