using System;

namespace Mongo.Context.Internal
{
    public struct MongoSetProperty
    {
        public MongoSetProperty(string name, string collectionName, Type entityType, IPropertySetter setter)
        {
            Name = name;
            EntityType = entityType;
            Setter = setter;
            CollectionName = collectionName;
        }

        public string Name { get; }
        public Type EntityType { get; }
        public IPropertySetter Setter { get; }
        public string CollectionName { get; }
    }
}
