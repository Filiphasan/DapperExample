using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper.Core.Models.Auth;
using Dapper.Core.Models.OptionModels;
using Dapper.Core.Services.interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dapper.Core.Services;

public class TokenService : ITokenService
{
    private readonly TokenConfigs _tokenConfigs;

    public TokenService(IOptions<TokenConfigs> tokenOptions)
    {
        _tokenConfigs = tokenOptions.Value;
    }

    public async Task<TokenResponseModel> CreateTokenAsync(TokenModel model)
    {
        return await GenerateTokenAsync(model);
    }

    public async Task<TokenResponseModel> CreateTokenAsync(TokenModel model, params Claim[] claims)
    {
        return await GenerateTokenAsync(model, claims);
    }

    public async Task<TokenResponseModel> CreateTokenAsync(TokenModel model, List<Claim> claims)
    {
        return await GenerateTokenAsync(model, claims);
    }

    private Task<TokenResponseModel> GenerateTokenAsync(TokenModel model, IEnumerable<Claim> claims = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfigs.SecurityKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claimList = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, model.Id),
            new(JwtRegisteredClaimNames.Name, model.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("Roles", string.Join(",", model.Roles ?? new List<string>()))
        };

        if (claims is not null)
        {
            claimList.AddRange(claims);
        }

        var jwtToken = new JwtSecurityToken(claims: claimList, notBefore: DateTime.Now,
            expires: DateTime.Now.AddSeconds(_tokenConfigs.ExpireTimeInSeconds), signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var response = new TokenResponseModel()
        {
            AccessToken = token,
            ExpireInSeconds = _tokenConfigs.ExpireTimeInSeconds,
            CreateDateTime = DateTime.Now,
        };
        return Task.FromResult(response);
    }
}