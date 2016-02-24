using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mongo.Context.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static string GetPropertyName<TObject>(this TObject type, Expression<Func<TObject, object>> propertyExpression)
        {
            return PropertyHelpers.GetPropertyName<TObject>(propertyExpression);
        }

        public static string GetPropertyShortName<TObject>(this TObject type, Expression<Func<TObject, object>> propertyExpression, bool includePropertyType = false)
        {
            return PropertyHelpers.GetPropertyShortName(propertyExpression, includePropertyType);
        }

        public static PropertyInfo GetAnyProperty(this Type type, string name)
        {
            var props = type.GetRuntimeProperties().Where(x => x.Name == name).ToList();
            if (props.Count() > 1)
            {
                throw new AmbiguousMatchException();
            }
            return props.SingleOrDefault();
        }

        public static Type UnwrapNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}
