using System.Security.Authentication;

namespace Dapper.Core.Services.interfaces;

public interface IHashService
{
    Task<string> GetHashedString(string password, HashAlgorithmType algorithmType = HashAlgorithmType.Sha512);
    Task<bool> CompareHashedString(string password, string hashedString);
}