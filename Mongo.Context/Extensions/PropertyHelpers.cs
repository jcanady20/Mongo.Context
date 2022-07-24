using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Mongo.Context.Extensions;

public static class PropertyHelpers
{
    private static string GetPropertyNameCore(Expression propertyExpression)
    {
        if (propertyExpression == null)
        {
            throw new ArgumentNullException("propertyExpression", "propertyExpression is null");
        }
        var memberExpression = propertyExpression as MemberExpression;
        if (memberExpression == null)
        {
            UnaryExpression unaryExpression = propertyExpression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
        }

        if (memberExpression != null && memberExpression.Member.MemberType == MemberTypes.Property)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("No Property reference expression was found.", "propertyExpression");
    }

    private static string GetPropertyTypeNameCore(Expression propertyExpression)
    {
        if (propertyExpression == null)
        {
            throw new ArgumentNullException("propertyExpression", "propertyExpression is null");
        }
        var memberExpression = propertyExpression as MemberExpression;
        if (memberExpression == null)
        {
            UnaryExpression unaryExpression = propertyExpression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
        }

        if (memberExpression != null && memberExpression.Member.MemberType == MemberTypes.Property)
        {
            var property = memberExpression.Member as PropertyInfo;
            if (property != null)
            {
                return property.PropertyType.Name;
            }
        }

        throw new ArgumentException("No Property reference expression was found.", "propertyExpression");
    }

    public static string GetPropertyName<TEntity>(Expression<Func<TEntity, object>> propertyExpression)
    {
        return GetPropertyNameCore(propertyExpression.Body);
    }

    public static string GetPropertyShortName<TEntity>(Expression<Func<TEntity, object>> propertyExpression, bool includePropertyType = false)
    {
        var result = String.Empty;
        var shortTypeName = String.Empty;

        var propertyName = GetPropertyNameCore(propertyExpression.Body);
        var propertyNameChars = propertyName
            .ToCharArray()
            .Where(c => Char.IsUpper(c) || Char.IsDigit(c));
        result = new String(propertyNameChars.ToArray()).ToLower();
        if (includePropertyType)
        {
            var propertyTypeName = GetPropertyTypeNameCore(propertyExpression.Body);
            var typeNameChars = propertyTypeName
                .ToCharArray()
                .Where(c => Char.IsUpper(c));
            shortTypeName = new String(typeNameChars.ToArray()).ToLower();
        }

        return result + shortTypeName;
    }
}
