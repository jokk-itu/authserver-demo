using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityProvider.Pages;

public class SignUpModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly SignInManager<IdentityUser> _signInManager;

    public SignUpModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = (userStore as IUserEmailStore<IdentityUser>)!;
        _signInManager = signInManager;
    }

    [BindProperty] public InputModel Input { get; set; }

    public IEnumerable<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [EmailAddress] public required string Email { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        [Compare(nameof(Password))] 
        public required string ConfirmPassword { get; set; }
    }

    public async Task OnGetAsync()
    {
        ExternalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ExternalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        if (ModelState.IsValid)
        {
            var user = new IdentityUser();
            await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            var identityResult = await _userManager.CreateAsync(user, Input.Password);

            if (identityResult.Succeeded)
            {
                // TODO an Email confirmation flow should be executed here
                await _emailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect("/");
            }

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // something went wrong, redisplay page to redisplay error messages
        return Page();
    }
}