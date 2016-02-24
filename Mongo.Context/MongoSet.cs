using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using Mongo.Context.Extensions;

namespace Mongo.Context
{
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
        protected MongoCollection<TEntity> Collection
        {
            get
            {
                return _context.GetDatabase().GetCollection<TEntity>(_collectionName);
            }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return Collection.FindAllAs<TEntity>().GetEnumerator();
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

        /// <summary>
        /// Inserts a new item
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <exception cref="MongoDB.Drive.MongoException" />
        public void Insert(TEntity item)
        {
            this.Collection.Insert<TEntity>(item);
        }

        /// <summary>
        /// Inserts an IEnumerable of items in a batch
        /// </summary>
        /// <param name="items">The items to insert</param>
        public void InsertBatch(IEnumerable<TEntity> items)
        {
            this.Collection.InsertBatch<TEntity>(items);
        }

        /// <summary>
        /// Saves the item
        /// </summary>
        /// <param name="item">The item to save</param>
        public void Save(TEntity item)
        {
            this.Collection.Save<TEntity>(item);
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

            var qry = Query.EQ(elName, BsonValue.Create(value));
            var result = Collection.Remove(qry);
            return result.DocumentsAffected;
        }

        /// <summary>
        /// Remove the item/s matching the criteria
        /// </summary>
        /// <param name="criteria">criteria expression</param>
        /// <returns>The number of records affected. If WriteConcern is unacknowledged -1 is returned</returns>
        public long Remove(Expression<Func<TEntity, bool>> criteria)
        {
            var queryable = this.Collection.AsQueryable<TEntity>().Where(criteria);
            var query = ((MongoQueryProvider)this.Provider).BuildMongoQuery<TEntity>((MongoQueryable<TEntity>)queryable);
            var result = this.Collection.Remove(query);
            return result.DocumentsAffected;
        }

        /// <summary>
        /// Removes all items
        /// </summary>
        /// <returns>The number of records affected. If WriteConcern is unacknowledged -1 is returned</returns>
        /// <remarks>Careful this deletes everything in the MongoSet/Collection!</remarks>
        public long RemoveAll()
        {
            var result = Collection.RemoveAll();
            return result.DocumentsAffected;
        }

        /// <summary>
        /// Update one property of an object.
        /// </summary>
        /// <typeparam name="TMember">The type of the property to be updated</typeparam>
        /// <param name="propertySelector">The property selector expression</param>
        /// <param name="value">New value of the property</param>
        /// <param name="criteria">Criteria to update documents based on</param>
        /// <returns>The number of records affected. If WriteConcern is unacknowledged -1 is returned</returns>
        public long Update<TMember>(Expression<Func<TEntity, TMember>> propertySelector, TMember value, Expression<Func<TEntity, bool>> criteria)
        {
            var updateBuilder = new UpdateBuilder<TEntity>();
            updateBuilder.Set<TMember>(propertySelector, value);
            return this.Update(updateBuilder, criteria);
        }

        /// <summary>
        /// Update with UpdateBuilder. Use MongoSet<typeparamref name"T"/>.Set().Set()... to build update
        /// </summary>
        /// <param name="update">The update object</param>
        /// <param name="criteria">Criteria to update documents based on</param>
        /// <returns></returns>
        private long Update(UpdateBuilder<TEntity> update, Expression<Func<TEntity, bool>> criteria)
        {
            var queryable = this.Collection.AsQueryable<TEntity>().Where(criteria);
            var query = ((MongoQueryProvider)this.Provider).BuildMongoQuery<TEntity>((MongoQueryable<TEntity>)queryable);
            var flags = UpdateFlags.Multi;
            var result = this.Collection.Update(query, update, flags);
            return result.DocumentsAffected;
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
            var value = classMap.IdMemberMap.Getter(item);
            var result = Collection.FindOneByIdAs<TEntity>(BsonValue.Create(value));
            return result != null;
        }
    }
}
