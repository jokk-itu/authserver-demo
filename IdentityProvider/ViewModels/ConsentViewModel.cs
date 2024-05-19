namespace IdentityProvider.ViewModels;

#nullable enable
public class ConsentViewModel
{
  public string ClientName { get; init; } = null!;
  public string GivenName { get; init; } = null!;
  public IEnumerable<object> Claims { get; init; } = [];
  public IEnumerable<object> Scopes { get; init; } = [];
  public string? TosUri { get; init; }
  public string? PolicyUri { get; init; }
  public string? ClientUri { get; init; }
  public string? LogoUri { get; init; }
}