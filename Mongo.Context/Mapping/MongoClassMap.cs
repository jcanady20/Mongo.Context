using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;

namespace Mongo.Context.Mapping
{
    public class MongoClassMap : BsonClassMap
    {
        private IEnumerable<MongoIndex> _indexes = new List<MongoIndex>();
        private string _collectionName;
        public MongoClassMap(Type classType) : base(classType)
        { }

        public string CollectionName
        {
            get
            {
                return _collectionName;
            }
            set
            {
                _collectionName = value;
            }
        }
        
        public IEnumerable<MongoIndex> Indexes
        {
            get
            {
                return _indexes;
            }
            set
            {
                _indexes = value;
            }
        }

        public void SetCollectionName(string collectionName)
        {
            _collectionName = collectionName;
        }
    }
}
