using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Bson;
using Mongo.Context.Extensions;

namespace Mongo.Context;
/// <summary>
/// MongoCollection<typeparamref name="T"/> wrapper to allow Linq and expression based access to
/// the collection without the need to switch between MongoQuery and Linq query operators.
/// </summary>
/// <typeparam name="T">Class implementing IMongoEntity</typeparam>
public class MongoSet<TEntity> : IMongoSet<TEntity> where TEntity : class
{
    private string _collectionName;
    private MongoContext _context;

    public MongoSet(MongoContext context)
    {
        _collectionName = Internal.NamePluralization.GetCollectionName(ElementType);
        _context = context;
    }

    public MongoSet(MongoContext context, string collectionName)
    {
        _collectionName = collectionName;
        _context = context;
    }

    /// <summary>
    /// Underlying MongoCollection for this MongoSet
    /// </summary>
    protected IMongoCollection<TEntity> Collection
    {
        get
        {
            return _context.GetDatabase().GetCollection<TEntity>(_collectionName);
        }
    }

    public string CollectionName { get { return _collectionName; } }

    public IEnumerator<TEntity> GetEnumerator()
    {
        return Collection.AsQueryable().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType
    {
        get
        {
            return typeof(TEntity);
        }
    }

    public Expression Expression
    {
        get
        {
            return Collection.AsQueryable().Expression;
        }
    }

    public IQueryProvider Provider
    {
        get
        {
            return Collection.AsQueryable().Provider;
        }
    }

    public TEntity FindOne(Expression<Func<TEntity, bool>> filter)
    {
        return this.Collection.Find(filter).FirstOrDefault();
    }

    /// <summary>
    /// Inserts a new item
    /// </summary>
    /// <param name="item">The item to insert</param>
    /// <exception cref="MongoDB.Drive.MongoException" />
    public void Insert(TEntity item)
    {
        this.Collection.InsertOne(item);
    }

    /// <summary>
    /// Inserts an IEnumerable of items in a batch
    /// </summary>
    /// <param name="items">The items to insert</param>
    public void InsertBatch(IEnumerable<TEntity> items)
    {
        this.Collection.InsertMany(items);
    }

    public void Save(Expression<Func<TEntity, bool>> filter, TEntity item)
    {
        this.Collection.ReplaceOne(filter, item);
    }

    /// <summary>
    /// Remove the item
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>The number of records affected. If WriteConcern is unacknowledged -1 is returned</returns>
    public long Remove(TEntity item)
    {
        var classMap = MongoBuilder.LookupClassMap(typeof(TEntity));
        if (classMap == null)
        {
            throw new ArgumentNullException(nameof(classMap));
        }
        var value = classMap.IdMemberMap.Getter(item);
        var elName = classMap.IdMemberMap.ElementName;
        var filter = new BsonDocument();
        filter.Add(elName, BsonValue.Create(value));
        var result = Collection.DeleteMany(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// Remove the item/s matching the criteria
    /// </summary>
    /// <param name="criteria">criteria expression</param>
    /// <returns>The number of records affected. If WriteConcern is unacknowledged -1 is returned</returns>
    public long Remove(Expression<Func<TEntity, bool>> criteria)
    {
        var result = this.Collection.DeleteMany(criteria);
        return result.DeletedCount;
    }

    /// <summary>
    /// Removes all items
    /// </summary>
    /// <returns>The number of records affected. If WriteConcern is unacknowledged -1 is returned</returns>
    /// <remarks>Careful this deletes everything in the MongoSet/Collection!</remarks>
    public long RemoveAll()
    {
        var filter = new BsonDocument();
        var result = Collection.DeleteMany(filter);
        return result.DeletedCount;
    }

    public string GetElementName(Expression<Func<TEntity, object>> propertyExpression)
    {
        var result = String.Empty;
        var classMap = MongoBuilder.LookupClassMap(typeof(TEntity));
        if (classMap == null)
        {
            throw new ArgumentException("No ClassMap reference was found.", nameof(propertyExpression));
        }
        var propertyName = PropertyHelpers.GetPropertyName<TEntity>(propertyExpression);
        var memberMap = classMap.GetMemberMap(propertyName);
        if (memberMap == null)
        {
            throw new ArgumentException("No MemberMap reference was found.", nameof(memberMap));
        }
        return memberMap.ElementName;
    }

    public bool Contains(TEntity item)
    {
        var classMap = MongoBuilder.LookupClassMap(typeof(TEntity));
        if (classMap == null)
        {
            throw new ArgumentNullException(nameof(classMap));
        }
        var idMap = classMap.IdMemberMap;
        if (idMap == null)
        {
            throw new ArgumentNullException(nameof(classMap));
        }
        var value = idMap.Getter(item);
        var filter = new BsonDocument();
        filter.Add(idMap.ElementName, BsonValue.Create(value));
        var result = Collection.Find<TEntity>(filter);
        return result != null;
    }
}
