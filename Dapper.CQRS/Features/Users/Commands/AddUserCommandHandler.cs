using System.Security.Authentication;
using Dapper.Core.Models;
using Dapper.Core.Services.interfaces;
using Dapper.CQRS.Models.Users;
using Dapper.Data.Entities;
using Dapper.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dapper.CQRS.Features.Users.Commands;

public class AddUserCommandHandler : BaseHandler<AddUserCommandHandler>, IBaseRequestHandler<AddUserCommand, AddUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;
    
    public AddUserCommandHandler(ILoggerFactory logger, IUserRepository userRepository, IHashService hashService) : base(logger)
    {
        _userRepository = userRepository;
        _hashService = hashService;
    }

    public async Task<GenericResponse<AddUserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userExist = await _userRepository.AnyAsync(x => x.Username == request.UserName);
            if (userExist)
            {
                return GenericResponse<AddUserResponse>.Error(400, "Username already exist.");
            }
            
            var response = new AddUserResponse();

            var hashedPassword = await _hashService.GetHashedString(request.Password);
            User newUser = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PublicId = Guid.NewGuid().ToString(),
                FullName = $"{request.FirstName} {request.LastName}",
                Username = request.UserName,
                Password = hashedPassword,
                CreateDate = DateTime.Now,
            };
            
            var result = await _userRepository.AddAsync(newUser);
            if (result == 0)
            {
                return GenericResponse<AddUserResponse>.Error(400, "User Add Operation Fail.");
            }
            
            response.Message = "User Successfully Added.";
            return GenericResponse<AddUserResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return HandleExceptionResponse<AddUserResponse>(ex);
        }
    }
}