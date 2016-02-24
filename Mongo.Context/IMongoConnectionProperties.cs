namespace Mongo.Context
{
    public interface IMongoConnectionProperties
    {
        string Server { get; }

        string DatabaseName { get; }

        string UserName { get; }

        string Password { get; }
    }
}
