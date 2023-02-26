using System.Security.Claims;
using Dapper.Core.Models.Auth;

namespace Dapper.Core.Services.interfaces;

public interface ITokenService
{
    Task<TokenResponseModel> CreateTokenAsync(TokenModel model);
    Task<TokenResponseModel> CreateTokenAsync(TokenModel model, params Claim[] claims);
    Task<TokenResponseModel> CreateTokenAsync(TokenModel model, List<Claim> claims);
}