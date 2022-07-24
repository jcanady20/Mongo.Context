using System.Collections.Generic;

namespace Mongo.Context.Internal;

public interface IMongoSetFinder
{
    IReadOnlyList<MongoSetProperty> FindSets(MongoContext context);
}

