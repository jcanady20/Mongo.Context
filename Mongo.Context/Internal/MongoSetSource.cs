using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Mongo.Context.Internal;

public class MongoSetSource : IMongoSetSource
{
    private static readonly MethodInfo _genericCreate = typeof(MongoSetSource).GetTypeInfo().GetDeclaredMethods("CreateConstructor").Single();
    private readonly ConcurrentDictionary<Type, Func<MongoContext, string, object>> _cache = new ConcurrentDictionary<Type, Func<MongoContext, string, object>>();
    public virtual object Create(MongoContext context, string collectionName, Type type)
    {
        return _cache.GetOrAdd(type, t => (Func<MongoContext, string, object>)_genericCreate.MakeGenericMethod(type).Invoke(null, null))(context, collectionName);
    }

    private static Func<MongoContext, string, object> CreateConstructor<TEntity>() where TEntity : class
    {
        return (c, n) => new MongoSet<TEntity>(c, n);
    }
}
