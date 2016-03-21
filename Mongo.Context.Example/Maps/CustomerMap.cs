using Mongo.Context.Mapping;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Context.Example.Maps
{
    public class CustomerMap : MongoClassMap<Entities.Customer>
    {
        public CustomerMap()
        {
            AutoMap();
            SetIgnoreExtraElements(true);
            MapIdProperty(x => x.Id)
                .SetSerializer(new StringSerializer(BsonType.ObjectId))
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        }
    }
}
