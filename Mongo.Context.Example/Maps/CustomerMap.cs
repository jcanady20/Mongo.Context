using Mongo.Context.Mapping;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Mongo.Context.Example.Maps
{
    public class CustomerMap : MongoClassMap<Entities.Customer>
    {
        public CustomerMap()
        {
            AutoMap();
            SetIgnoreExtraElements(true);
            MapIdProperty(x => x.Id)
                .SetRepresentation(MongoDB.Bson.BsonType.ObjectId)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        }
    }
}
