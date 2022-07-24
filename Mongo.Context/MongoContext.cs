using Mongo.Context.Extensions;
using Mongo.Context.Mapping;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Mongo.Context;
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

    public IMongoDatabase GetDatabase()
    {
        return _client.GetDatabase(_databaseName);
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
            var options = new CreateIndexOptions();
            var indexBuilder = Builders<BsonDocument>.IndexKeys;
            var keydefs = new List<IndexKeysDefinition<BsonDocument>>();
            foreach(var key in idx.Keys)
            {
                keydefs.Add((idx.Descending) ? indexBuilder.Descending(key) : indexBuilder.Ascending(key));
            }
            var indexDefinition = indexBuilder.Combine(keydefs);
            options.Unique = idx.Unique;

            if (idx.TimeToLive > -1)
            {
                options.ExpireAfter = TimeSpan.FromSeconds(idx.TimeToLive);
            }

            var collection = GetDatabase().GetCollection<BsonDocument>(collectionName);
            var indexModel = new CreateIndexModel<BsonDocument>(indexDefinition, options);
            collection.Indexes.CreateOne(indexModel);
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
