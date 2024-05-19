using System.Net;
using IdentityProvider.Constants;
using IdentityProvider.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers.Abstracts;

public abstract class OAuthControllerBase : Controller
{
    protected IActionResult CreatedOAuthResult(string locationPath, object createdEntity)
    {
      if (!Uri.TryCreate($"{Request.Scheme}://{Request.Host}/{locationPath}", UriKind.Absolute, out var location))
      {
        throw new ArgumentException("is not a well-formed path", nameof(locationPath));
      }

      return Created(location, createdEntity);
    }

    protected IActionResult BadOAuthResult(string? error, string? errorDescription)
    {
        var response = new ErrorResponse();
        if (!string.IsNullOrWhiteSpace(error))
        {
            response.Error = error;
        }

        if (!string.IsNullOrWhiteSpace(errorDescription))
        {
            response.ErrorDescription = errorDescription;
        }

        return BadRequest(response);
    }

    protected IActionResult AuthorizationCodeFormPostResult(string redirectUri, string state, string code)
    {
        if (!Uri.IsWellFormedUriString(redirectUri, UriKind.Absolute))
        {
            throw new ArgumentException($"{nameof(redirectUri)} must be a well formed uri");
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            throw new ArgumentException($"{nameof(state)} must not be null or whitespace");
        }

        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentType = MimeTypeConstants.Html,
            Content = FormPostBuilder.BuildAuthorizationCodeResponse(redirectUri, state, code, "")
        };
    }

    protected IActionResult LogoutRedirectResult(string redirectUri, string state)
    {
      if (!Uri.IsWellFormedUriString(redirectUri, UriKind.Absolute))
      {
        throw new ArgumentException($"{nameof(redirectUri)} must not be null or whitespace");
      }

      if (string.IsNullOrWhiteSpace(state))
      {
        throw new ArgumentException($"{nameof(state)} must not be null or whitespace");
      }

      var query = new QueryBuilder().ToQueryString();

      return Redirect($"{redirectUri}{query}");
    }

    protected IActionResult ErrorFormPostResult(string redirectUri, string state, string? error, string? errorDescription)
    {
        if (!Uri.IsWellFormedUriString(redirectUri, UriKind.Absolute))
        {
            throw new ArgumentException($"{nameof(redirectUri)} must be a well formed uri");
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            throw new ArgumentException($"{nameof(state)} must not be null or whitespace");
        }

        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            ContentType = MimeTypeConstants.Html,
            Content = FormPostBuilder.BuildErrorResponse(redirectUri, state, error, errorDescription, "")
        };
    }
}