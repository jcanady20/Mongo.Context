using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mongo.Context;
public interface IMongoSet<TEntity> : IQueryable<TEntity>, IQueryable, IEnumerable<TEntity> where TEntity : class
{
    string CollectionName { get; }
    TEntity FindOne(Expression<Func<TEntity, bool>> filter);
    void Insert(TEntity item);
    void InsertBatch(IEnumerable<TEntity> items);
    long Remove(Expression<Func<TEntity, bool>> criteria);
    long RemoveAll();
    void Save(Expression<Func<TEntity, bool>> criteria, TEntity item);
    bool Contains(TEntity item);
}
