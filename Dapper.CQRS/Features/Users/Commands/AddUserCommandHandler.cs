using Dapper.Core.Models;
using Dapper.CQRS.Models.Users;
using Dapper.Data.Entities;
using Dapper.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dapper.CQRS.Features.Users.Commands;

public class AddUserCommandHandler : BaseHandler<AddUserCommandHandler>, IBaseRequestHandler<AddUserCommand, AddUserResponse>
{
    private readonly IUserRepository _userRepository;
    
    public AddUserCommandHandler(ILoggerFactory logger, IUserRepository userRepository) : base(logger)
    {
        _userRepository = userRepository;
    }

    public async Task<GenericResponse<AddUserResponse>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userExist = await _userRepository.GetLastOrDefaultAsync(x => x.Username == request.UserName);
            if (userExist != null)
            {
                return GenericResponse<AddUserResponse>.Error(400, "Username already exist.");
            }
            
            var response = new AddUserResponse();

            User newUser = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = $"{request.FirstName} {request.LastName}",
                Username = request.UserName,
                Password = request.Password, //TODO: Hash or Encrypt Password
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
            var result = GenericResponse<AddUserResponse>.Error(500, "Somethings Went Wrong!");
            _logger.LogError(ex, "Error RequestId: {RequestId}", result.Uuid);
            return result;
        }
    }
}