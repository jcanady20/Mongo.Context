using Mongo.Context.Mapping;
using System;
using System.Collections.Generic;

namespace Mongo.Context.Internal;

public interface IMongoSetInitializer
{
    void InitializeSets(MongoContext context, IDictionary<Type, MongoClassMap> classMaps);
    MongoSet<TEntity> CreateSet<TEntity>(MongoContext context, string collectionName) where TEntity : class;
}

