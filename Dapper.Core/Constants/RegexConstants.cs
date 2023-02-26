namespace Dapper.Core.Constants;

public struct RegexConstants
{
    public const string PasswordRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[*\\-&+.,])[A-Za-z\\d*\\-&+.,?=#$:;]{6,20}$";
    public const string PasswordRegexMessage = "Password contains at least one lower and upper letter, one number, one special character such as (-&+.,?=#$:;). Password length must between 6 and 20.";
}