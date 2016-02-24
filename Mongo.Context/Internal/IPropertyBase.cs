namespace Mongo.Context.Internal
{
    public interface IPropertyBase
    {
        string Name { get; }
        IEntityType DeclaringEntityType { get; }
    }
}
