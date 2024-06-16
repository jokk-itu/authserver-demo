using AuthServer.Constants;
using AuthServer.Core.Discovery;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Options;

public class ConfigureDiscoveryDocumentOptions : IConfigureOptions<DiscoveryDocument>
{
    private readonly IConfiguration _configuration;

    public ConfigureDiscoveryDocumentOptions(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DiscoveryDocument options)
    {
        var identitySection = _configuration.GetSection("Identity");

        options.Issuer = identitySection.GetValue<string>("Issuer")!;
        options.ServiceDocumentation = identitySection.GetValue<string>("ServiceDocumentation")!;
        options.OpPolicyUri = identitySection.GetValue<string>("TosUri");
        options.OpTosUri = identitySection.GetValue<string>("PolicyUri");
        options.ClaimsSupported = ClaimNameConstants.SupportedEndUserClaims;
        options.ScopesSupported = ["weather:read"];
        options.RequestParameterSupported = true;
        options.RequestUriParameterSupported = true;

        ICollection<string> signingAlgorithms = [JwsAlgConstants.RsaSha256, JwsAlgConstants.EcdsaSha256, JwsAlgConstants.RsaSsaPssSha256];
        ICollection<string> encryptionAlgorithms = [JweAlgConstants.EcdhEsA128KW, JweAlgConstants.RsaOAEP, JweAlgConstants.RsaPKCS1];
        ICollection<string> encoderAlgorithms = [JweEncConstants.Aes128CbcHmacSha256];

        options.TokenEndpointAuthSigningAlgValuesSupported = signingAlgorithms;
        options.IdTokenSigningAlgValuesSupported = signingAlgorithms;
        options.IntrospectionEndpointAuthSigningAlgValuesSupported = signingAlgorithms;
        options.RequestObjectSigningAlgValuesSupported = signingAlgorithms;
        options.RevocationEndpointAuthSigningAlgValuesSupported = signingAlgorithms;
        options.UserinfoSigningAlgValuesSupported = signingAlgorithms;

        options.IdTokenEncryptionAlgValuesSupported = encryptionAlgorithms;
        options.RequestObjectEncryptionAlgValuesSupported = encryptionAlgorithms;
        options.UserinfoEncryptionAlgValuesSupported = encryptionAlgorithms;

        options.IdTokenEncryptionEncValuesSupported = encoderAlgorithms;
        options.RequestObjectEncryptionEncValuesSupported = encoderAlgorithms;
        options.UserinfoEncryptionEncValuesSupported = encoderAlgorithms;
    }
}