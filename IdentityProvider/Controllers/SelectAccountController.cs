using IdentityProvider.Constants;
using IdentityProvider.Controllers.Abstracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[Route("connect/[controller]")]
public class SelectAccountController : OAuthControllerBase
{
  [HttpGet]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  public IActionResult Index()
  {
    return View("Index");
  }

  [HttpPost]
  [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
  [Consumes(MimeTypeConstants.FormUrlEncoded)]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Post(CancellationToken cancellationToken = default)
  {
      // TODO silent login using the chosen UserId
      // TODO if loginrequired, then redirect to login
      // TODO if success, then return authorization code
      throw new NotImplementedException();
  }
}