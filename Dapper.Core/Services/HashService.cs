using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Dapper.Core.Services.interfaces;

namespace Dapper.Core.Services;

public class HashService : IHashService
{
    public Task<string> GetHashedString(string password, HashAlgorithmType algorithmType = HashAlgorithmType.Sha512)
    {
        var message = Encoding.UTF8.GetBytes(password);
        using var alg = GetHashAlgorithm(algorithmType);

        string hex = "";
        var hashValue = alg.ComputeHash(message);
        hex = Convert.ToBase64String(hashValue);

        return Task.FromResult(hex);
    }

    public async Task<bool> CompareHashedString(string password, string hashedString)
    {
        var hashedPassword = await GetHashedString(password);
        return hashedPassword.Equals(hashedString);
    }

    private static HashAlgorithm GetHashAlgorithm(HashAlgorithmType type) =>
        type switch
        {
            HashAlgorithmType.Md5 => MD5.Create(),
            HashAlgorithmType.Sha1 => SHA1.Create(),
            HashAlgorithmType.Sha256 => SHA256.Create(),
            HashAlgorithmType.Sha384 => SHA384.Create(),
            HashAlgorithmType.Sha512 => SHA512.Create(),
            _ => throw new CryptographicException("Unsupported HashAlgorithmType")
        };
}