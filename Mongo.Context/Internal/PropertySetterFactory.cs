using System;
using System.Reflection;

namespace Mongo.Context.Internal;

public class PropertySetterFactory : AccessorFactory<IPropertySetter>
{
    public override IPropertySetter Create(PropertyInfo property)
    {
        var types = new[] { property.DeclaringType, property.PropertyType };
        return (IPropertySetter)Activator.CreateInstance(typeof(PropertySetter<,>).MakeGenericType(types), property.SetMethod.CreateDelegate(typeof(Action<,>).MakeGenericType(types)));
    }
}
