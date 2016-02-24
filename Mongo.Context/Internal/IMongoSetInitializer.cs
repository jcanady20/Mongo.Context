using System.Linq;

namespace Mongo.Context.Internal
{
    public interface IMongoSetInitializer
    {
        void InitializeSets(MongoContext context);
        MongoSet<TEntity> CreateSet<TEntity>(MongoContext context, string collectionName) where TEntity : class;
    }
}
