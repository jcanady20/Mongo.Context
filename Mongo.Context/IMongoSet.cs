using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mongo.Context
{
    public interface IMongoSet<TEntity> : IQueryable<TEntity>, IQueryable, IEnumerable<TEntity> where TEntity : class
    {
        void Insert(TEntity item);
        void InsertBatch(IEnumerable<TEntity> items);
        long Remove(Expression<Func<TEntity, bool>> criteria);
        long RemoveAll();
        long Update<TMember>(Expression<Func<TEntity, TMember>> propertySelector, TMember value, Expression<Func<TEntity, bool>> criteria);
        void Save(TEntity item);
        bool Contains(TEntity item);
    }
}
