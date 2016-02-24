namespace Mongo.Context.Example.Entities
{
    public class Address
    {
        public Address()
        {
            Enable = true;
        }
        public string Id { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public bool Enable { get; set; }
    }
}
