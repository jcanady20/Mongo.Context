using Mongo.Context.Extensions;
using Mongo.Context.Mapping;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mongo.Context
{
    /// <summary>
    /// Mongo Context class similar to EntityFrameworks DbContext.
    /// </summary>
    public class MongoContext : IDisposable
    {
        private string _databaseName;
        private MongoUrlBuilder _connectionStringBuilder;
        private MongoClient _client;

        public MongoContext(string connectionString, string databaseName = null)
        {
            _connectionStringBuilder = new MongoUrlBuilder(connectionString);
            _connectionStringBuilder.DatabaseName = DatabaseName(databaseName);
            _client = new MongoClient(_connectionStringBuilder.ToMongoUrl());
            RegisterClasses();
        }

        public MongoDatabase GetDatabase()
        {
            return _client.GetServer().GetDatabase(_databaseName);
        }

        protected virtual void OnRegisterClasses(MongoBuilder mongoBuilder)
        { }

        private void RegisterClasses()
        {
            var builder = new MongoBuilder(this);
            if (!builder.IsFrozen)
            {
                OnRegisterClasses(builder);
            }
            builder.InitializeSets();
            builder.InitializeIndexes();
        }

        private string DatabaseName(string databaseName = null)
        {
            if (String.IsNullOrEmpty(databaseName) == false)
            {
                return _databaseName = databaseName;
            }

            if (String.IsNullOrEmpty(_connectionStringBuilder.DatabaseName) == false)
            {
                return _databaseName = _connectionStringBuilder.DatabaseName;
            }

            if (string.IsNullOrEmpty(_databaseName))
            {
                throw new ArgumentException("DatabaseName cannot be null or empty");
            }
            return _databaseName;
        }

        internal void EnsureIndexes(string collectionName, IEnumerable<MongoIndex> indexes)
        {
            if (!indexes.HasItems())
            {
                return;
            }

            foreach (var idx in indexes)
            {
                var indexKeysBuilder = new IndexKeysBuilder();
                IndexOptionsBuilder indexOptionsBuilder = new IndexOptionsBuilder();
                if (idx.Unique)
                {
                    indexOptionsBuilder.SetUnique(idx.Unique);
                }
                if (idx.TimeToLive > -1)
                {
                    TimeSpan timeToLive = new TimeSpan(0, 0, idx.TimeToLive);
                    indexOptionsBuilder.SetTimeToLive(timeToLive);
                }

                var collection = GetDatabase().GetCollection(collectionName);
                collection.CreateIndex(idx.Desending ? indexKeysBuilder.Descending(idx.Keys.ToArray()) : indexKeysBuilder.Ascending(idx.Keys.ToArray()), indexOptionsBuilder);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionStringBuilder = null;
                _databaseName = null;
            }
        }
    }
}
