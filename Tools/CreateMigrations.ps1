Push-Location ../IdentityProvider
dotnet build
dotnet ef migrations add $args[0] -c AuthorizationDbContext -s IdentityProvider.csproj -o "Migrations/AuthServer/"
dotnet ef migrations add $args[0] -c AuthenticationDbContext -s IdentityProvider.csproj -o "Migrations/Identity/" 
Pop-Location