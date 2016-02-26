using System;

namespace Mongo.Context
{
    public static class MongoId
    {
        public static string GenerateId()
        {
            return GenerateId(DateTime.UtcNow);
        }
        public static string GenerateId(DateTime dateTime)
        {
            return MongoDB.Bson.ObjectId.GenerateNewId(dateTime).ToString();
        }
        public static string Empty()
        {
            return MongoDB.Bson.ObjectId.Empty.ToString();
        }
    }
}
