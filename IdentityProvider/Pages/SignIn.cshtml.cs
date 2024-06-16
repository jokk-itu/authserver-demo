using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages;

public class SignInModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public SignInModel(
        SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }
    public string ReturnUrl { get; set; }

    public class InputModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl)
    {
        // Make sure a fresh login is made
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        ExternalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var identityResult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, false);
            if (identityResult.Succeeded)
            {
                return Redirect(ReturnUrl);
            }
            else if (identityResult.RequiresTwoFactor)
            {
                // TODO redirect to 2fa
                throw new NotImplementedException();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return Page();
            }
        }

        // something went wrong, redisplay page to redisplay error messages
        return Page();
    }
}