using System.Security.Claims;
using AuthServer.Core.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Services;

public class UserClaimService : IUserClaimService
{
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserClaimStore<IdentityUser> _userClaimStore;
    private readonly ILogger<UserClaimService> _logger;

    public UserClaimService(
        IUserStore<IdentityUser> userStore,
        ILogger<UserClaimService> logger)
    {
        _userStore = userStore;
        _userClaimStore = (userStore as IUserClaimStore<IdentityUser>)!;
        _logger = logger;
    }

    public async Task<IEnumerable<Claim>> GetClaims(string subjectIdentifier, CancellationToken cancellationToken)
    {
        var user = await _userStore.FindByIdAsync(subjectIdentifier, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {SubjectIdentifier} was not found", subjectIdentifier);
            return [];
        }

        return await _userClaimStore.GetClaimsAsync(user, cancellationToken);
    }
}