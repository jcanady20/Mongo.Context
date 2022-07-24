using System;

namespace Mongo.Context.Internal;

public class PropertySetter<TEntity, TValue> : IPropertySetter where TEntity : class
{
    private readonly Action<TEntity, TValue> _setter;
    public PropertySetter(Action<TEntity, TValue> setter)
    {
        _setter = setter;
    }

    public virtual void SetValue(object instance, object value)
    {
        _setter((TEntity)instance, (TValue)value);
    }
}
