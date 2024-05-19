using IdentityProvider.Controllers.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers;

[Route("connect/end-session")]
public class EndSessionController : OAuthControllerBase
{
  [HttpGet]
  public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
  {
    // TODO return a new logout view
    throw new NotImplementedException();
  }
}