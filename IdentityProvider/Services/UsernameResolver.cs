using AuthServer.Introspection;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public class UsernameResolver : IUsernameResolver
{
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly ILogger<UsernameResolver> _logger;

    public UsernameResolver(
        IUserStore<IdentityUser> userStore,
        ILogger<UsernameResolver> logger)
    {
        _userStore = userStore;
        _logger = logger;
    }

    public async Task<string?> GetUsername(string subjectIdentifier)
    {
        var user = await _userStore.FindByIdAsync(subjectIdentifier, CancellationToken.None);
        if (user is null)
        {
            _logger.LogWarning("User {SubjectIdentifier} was not found", subjectIdentifier);
        }

        return user?.UserName;
    }
}