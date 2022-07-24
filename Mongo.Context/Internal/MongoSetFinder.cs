using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Mongo.Context.Internal;
public class MongoSetFinder : IMongoSetFinder
{
    private readonly ConcurrentDictionary<Type, IReadOnlyList<MongoSetProperty>> _cache = new ConcurrentDictionary<Type, IReadOnlyList<MongoSetProperty>>();

    public virtual IReadOnlyList<MongoSetProperty> FindSets(MongoContext context)
    {
        return _cache.GetOrAdd(context.GetType(), FindSets);
    }

    private static MongoSetProperty[] FindSets(Type contextType)
    {
        var setterFactory = new PropertySetterFactory();

        return contextType.GetRuntimeProperties()
            .Where(
                x => !(x.GetMethod ?? x.SetMethod).IsStatic
                    && !x.GetIndexParameters().Any()
                    && (x.DeclaringType != typeof(MongoContext))
                    && x.PropertyType.GetTypeInfo().IsGenericType
                    && (x.PropertyType.GetGenericTypeDefinition() == typeof(MongoSet<>))
            )
            .OrderBy(x => x.Name)
            .Select(r =>
            new MongoSetProperty(
                r.Name,
                NamePluralization.GetCollectionName(r.PropertyType),
                r.PropertyType.GetTypeInfo().GenericTypeArguments.Single(),
                r.SetMethod == null ? null : setterFactory.Create(r))
            ).ToArray();
    }
}

