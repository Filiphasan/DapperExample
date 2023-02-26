namespace Dapper.Core.Models.Auth;

public class TokenModel
{
    public string Id { get; set; }
    public string Username { get; set; }
    public IEnumerable<string> Roles { get; set; }
}

public class TokenResponseModel
{
    public string AccessToken { get; set; }
    public int ExpireInSeconds { get; set; }
    public DateTime CreateDateTime { get; set; }
}