using System;

namespace Mongo.Context.Internal;

public interface IMongoSetSource
{
    object Create(MongoContext context, string collectionName, Type type);
}

