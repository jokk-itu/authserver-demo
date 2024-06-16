using System.Security.Cryptography;
using AuthServer.Core.Discovery;
using AuthServer.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IdentityProvider.Options;

public class ConfigureJwksDocumentOptions : IConfigureOptions<JwksDocument>
{
    public void Configure(JwksDocument options)
    {
        var ecdsa = ECDsa.Create();
        var rsa = RSA.Create(3072);

        var ecdsaSecurityKey = new ECDsaSecurityKey(ecdsa);
        var rsaSecurityKey = new RsaSecurityKey(rsa);

        options.EncryptionKeys =
        [
            new JwksDocument.EncryptionKey(ecdsaSecurityKey, EncryptionAlg.EcdhEsA128KW),
            new JwksDocument.EncryptionKey(ecdsaSecurityKey, EncryptionAlg.EcdhEsA192KW),
            new JwksDocument.EncryptionKey(ecdsaSecurityKey, EncryptionAlg.EcdhEsA256KW),
            new JwksDocument.EncryptionKey(rsaSecurityKey, EncryptionAlg.RsaOAEP),
            new JwksDocument.EncryptionKey(rsaSecurityKey, EncryptionAlg.RsaPKCS1),
        ];
        options.SigningKeys =
        [
            new JwksDocument.SigningKey(ecdsaSecurityKey, SigningAlg.EcdsaSha256),
            new JwksDocument.SigningKey(ecdsaSecurityKey, SigningAlg.EcdsaSha384),
            new JwksDocument.SigningKey(ecdsaSecurityKey, SigningAlg.EcdsaSha512),
            new JwksDocument.SigningKey(rsaSecurityKey, SigningAlg.RsaSha256),
            new JwksDocument.SigningKey(rsaSecurityKey, SigningAlg.RsaSha384),
            new JwksDocument.SigningKey(rsaSecurityKey, SigningAlg.RsaSha512),
            new JwksDocument.SigningKey(rsaSecurityKey, SigningAlg.RsaSsaPssSha256),
            new JwksDocument.SigningKey(rsaSecurityKey, SigningAlg.RsaSsaPssSha384),
            new JwksDocument.SigningKey(rsaSecurityKey, SigningAlg.RsaSsaPssSha512),
        ];
    }
}