using System.Linq.Expressions;
using Dapper.Core.Data.Interfaces;

namespace Dapper.Data.Helpers;

public static class DbExpressionHelper
{
    public static string GetExpressionSql<TEntity>(Expression<Func<TEntity, bool>>? predicate)
        where TEntity : class, IDbEntity, new()
    {
        if (predicate?.Body is BinaryExpression binaryExpression)
        {
            return GetBinaryExpressionSql<TEntity>(binaryExpression);
        }

        return predicate?.ToString() ?? string.Empty;
    }

    private static string GetBinaryExpressionSql<TEntity>(BinaryExpression binaryExpression)
        where TEntity : class, IDbEntity, new()
    {
        var left = binaryExpression.Left as MemberExpression;
        var right = binaryExpression.Right as ConstantExpression;

        var dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(left.Member.Name);
        var value = right?.Value;

        var operatorString = GetOperatorString(binaryExpression.NodeType);

        return $"{dbColumnName} {operatorString} {value}";
    }

    private static string GetOperatorString(ExpressionType expressionType)
        => expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            _ => ""
        };
}