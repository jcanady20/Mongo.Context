using Mongo.Context.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mongo.Context.Internal;

public class MongoSetInitializer : IMongoSetInitializer
{
    private readonly IMongoSetFinder _setFinder;
    private readonly IMongoSetSource _setSource;
    public MongoSetInitializer(IMongoSetFinder setFinder, IMongoSetSource setSource)
    {
        _setFinder = setFinder;
        _setSource = setSource;
    }

    public MongoSet<TEntity> CreateSet<TEntity>(MongoContext context, string collectionName) where TEntity : class
    {
        return (MongoSet<TEntity>)_setSource.Create(context, collectionName,  typeof(TEntity));
    }

    public void InitializeSets(MongoContext context, IDictionary<Type, MongoClassMap> classMaps)
    {
        foreach (var setInfo in _setFinder.FindSets(context).Where(x => x.Setter != null))
        {
            var cm = classMaps[setInfo.EntityType];
            setInfo.Setter.SetValue(context, _setSource.Create(context, cm?.CollectionName ?? setInfo.CollectionName, setInfo.EntityType));
        }
    }
}
