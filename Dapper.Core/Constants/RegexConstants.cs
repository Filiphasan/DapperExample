namespace Dapper.Core.Constants;

public struct RegexConstants
{
    public const string PasswordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[*\\-&+.,])[A-Za-z\\d*\\-&+.,?=#$:;]{6,20}$";
}