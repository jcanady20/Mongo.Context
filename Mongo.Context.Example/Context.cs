using Mongo.Context.Mapping;

namespace Mongo.Context.Example
{
    public class Context : MongoContext
    {
        public Context(string connectionString) : base(connectionString) { }

        public MongoSet<Entities.Contact> Contacts { get; set; }
        public MongoSet<Entities.Customer> Customers { get; set; }

        protected override void OnRegisterClasses(MongoBuilder mongoBuilder)
        {
            mongoBuilder.FromAssembly(typeof(Context).Assembly);

            //  Manual Mapping for an Element
            var clsmap = mongoBuilder
                .Entry<Entities.Contact>();
            clsmap.SetCollectionName("SuperContacts");
            clsmap.MapProperty(x => x.Gender)
                .SetElementName("g");
            //  Adding an Index to a Collection
            mongoBuilder.Entry<Entities.Contact>()
                .Indexes = new[] {
                    new Mapping.MongoIndex() { Keys = new[] { "Name" } },
                    new Mapping.MongoIndex() { Keys = new[] { "Phone" } }
                };
        }
    }
}
