using Dapper.Core.Models;
using Dapper.CQRS.Models.Users;
using Microsoft.Extensions.Logging;

namespace Dapper.CQRS.Features.Users.Commands;

public class UpdateUserPasswordCommandHandler : BaseHandler<UpdateUserPasswordCommandHandler>, IBaseRequestHandler<UpdateUserPasswordCommand, UpdateUserPasswordResponse>
{
    public UpdateUserPasswordCommandHandler(ILoggerFactory logger) : base(logger)
    {
    }

    public async Task<GenericResponse<UpdateUserPasswordResponse>> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return null;
        }
        catch (Exception ex)
        {
            return HandleExceptionResponse<UpdateUserPasswordResponse>(ex);
        }
    }
}