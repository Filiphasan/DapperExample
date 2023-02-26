namespace Dapper.Core.Models;

public class GenericResponse<TResult> where TResult: class, new()
{
    public string Uuid { get; set; } = Guid.NewGuid().ToString();
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public TResult Data { get; set; }
    public IEnumerable<string> Errors { get; set; }
    public IEnumerable<string> ValidationErrors { get; set; }

    public GenericResponse()
    {
    }

    public GenericResponse(int statusCode, string errorMessage)
    {
        IsSuccess = false;
        StatusCode = statusCode;
        Errors = new List<string>() { errorMessage };
        Data = null;
    }

    public GenericResponse(int statusCode, string[] errorMessages)
    {
        IsSuccess = false;
        StatusCode = statusCode;
        Errors = errorMessages;
        Data = null;
    }

    public GenericResponse(int statusCode, bool isSuccess, TResult result)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Data = result;
        Errors = null;
    }

    public static GenericResponse<TResult> Error(int statusCode, string errorMessage)
    {
        return new GenericResponse<TResult>(statusCode, errorMessage);
    }
    
    public static GenericResponse<TResult> Error(int statusCode, params string[] errorMessages)
    {
        return new GenericResponse<TResult>(statusCode, errorMessages);
    }
    
    public static GenericResponse<TResult> Error(int statusCode, IEnumerable<string> errorMessages)
    {
        return new GenericResponse<TResult>(statusCode, errorMessages.ToArray());
    }

    public static GenericResponse<TResult> Success(TResult result)
    {
        return new GenericResponse<TResult>(200, true, result);
    }

    public static GenericResponse<TResult> Success(int statusCode ,TResult result)
    {
        return new GenericResponse<TResult>(statusCode, true, result);
    }
}