using Dapper.Core.Models;
using MediatR;

namespace Dapper.CQRS;

public interface IBaseRequestHandler<in TRequest, TResponse> : IRequestHandler<TRequest, GenericResponse<TResponse>> where TRequest : IRequest<GenericResponse<TResponse>> where TResponse : class, new()
{
    
}