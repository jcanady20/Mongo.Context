using Mongo.Context.Mapping;

namespace Mongo.Context
{
    public interface IMongoBuilder
    {
        bool IsFrozen { get; }

        MongoClassMap<T> Entry<T>();
    }
}
