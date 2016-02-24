using System;

namespace Mongo.Context.Internal
{
    public interface IEntityType
    {
        string Name { get; }
        IEntityType BaseType { get; }
        Type ClrType { get; }
    }
}
