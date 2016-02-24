namespace Mongo.Context.Internal
{
    public interface IPropertySetter
    {
        void SetValue(object instance, object value);
    }
}
