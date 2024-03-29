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
        if (binaryExpression.NodeType is ExpressionType.AndAlso or ExpressionType.OrElse)
        {
            var leftTemp = GetBinaryExpressionSql<TEntity>(binaryExpression.Left as BinaryExpression);
            var rightTemp = GetBinaryExpressionSql<TEntity>(binaryExpression.Right as BinaryExpression);
            var operatorString = GetOperatorString(binaryExpression.NodeType);

            return $"({leftTemp} {operatorString} {rightTemp})";
        }
        
        var left = binaryExpression.Left as MemberExpression;
        var right = binaryExpression.Right;

        if (left != null)
        {
            var dbColumnName = DbPropertyHelper.GetDatabaseColumnName<TEntity>(left.Member.Name);
            var value = GetRightValueForExpressionSql(right);
            var operatorString = GetOperatorString(binaryExpression.NodeType);

            return $"{dbColumnName} {operatorString} {value}";
        }

        return string.Empty;
    }

    private static string GetRightValueForExpressionSql(Expression expression)
    {
        var f = Expression.Lambda(expression).Compile();
        var objectValue = f.DynamicInvoke();
        var type = objectValue.GetType().Name.ToLower();
        var value = type switch
        {
            "string" => $"\'{objectValue}\'",
            "datetime" => $"'{Convert.ToDateTime(objectValue).ToString("yyyy-MM-dd HH:mm:ss")}'",
            _ => ""
        };

        return value;
    }

    private static string GetOperatorString(ExpressionType expressionType)
        => expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new InvalidOperationException("Unsupported ExpressionType")
        };
}