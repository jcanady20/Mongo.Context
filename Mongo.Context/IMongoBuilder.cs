using Mongo.Context.Mapping;
using System.Reflection;

namespace Mongo.Context;
public interface IMongoBuilder
{
    bool IsFrozen { get; }

    MongoClassMap<T> Entry<T>();

    void FromAssembly(Assembly assembly);

    void InitializeIndexes();

    void InitializeSets();
}
