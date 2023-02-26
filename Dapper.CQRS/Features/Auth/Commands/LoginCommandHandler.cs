using Dapper.Core.Models;
using Dapper.Core.Models.Auth;
using Dapper.Core.Services.interfaces;
using Dapper.CQRS.Models.Auth;
using Dapper.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dapper.CQRS.Features.Auth.Commands;

public class LoginCommandHandler : BaseHandler<LoginCommandHandler>, IBaseRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(ILoggerFactory logger, IUserRepository userRepository, IHashService hashService, ITokenService tokenService) :
        base(logger)
    {
        _userRepository = userRepository;
        _hashService = hashService;
        _tokenService = tokenService;
    }

    public async Task<GenericResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(x => x.Username == request.Username);
            if (user is null)
            {
                return GenericResponse<LoginResponse>.Error(400, "User not found.");
            }

            var isPasswordValid = await _hashService.CompareHashedString(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return GenericResponse<LoginResponse>.Error(400, "User password is wrong.");
            }

            var tokenModel = new TokenModel()
            {
                Id = user.PublicId,
                Username = user.Username,
            };
            var token = await _tokenService.CreateTokenAsync(tokenModel);

            var response = new LoginResponse()
            {
                AccessToken = token.AccessToken,
                ExpireInSeconds = token.ExpireInSeconds,
                CreateDateTime = token.CreateDateTime,
            };
            return GenericResponse<LoginResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return HandleExceptionResponse<LoginResponse>(ex);
        }
    }
}