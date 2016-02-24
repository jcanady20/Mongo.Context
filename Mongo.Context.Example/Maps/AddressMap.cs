using Mongo.Context.Mapping;
using MongoDB.Bson.Serialization.IdGenerators;


namespace Mongo.Context.Example.Maps
{
    public class AddressMap : MongoClassMap<Entities.Address>
    {
        public AddressMap()
        {
            AutoMap();
            SetIgnoreExtraElements(true);
            MapIdProperty(x => x.Id)
                .SetRepresentation(MongoDB.Bson.BsonType.ObjectId)
                .SetIdGenerator(StringObjectIdGenerator.Instance);
        }
    }
}
