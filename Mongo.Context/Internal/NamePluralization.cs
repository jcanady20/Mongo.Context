using System;
using System.Data.Entity.Design.PluralizationServices;

namespace Mongo.Context.Internal
{
    internal static class NamePluralization
    {
        internal static string GetCollectionName(Type elementType)
        {
            if(elementType.IsGenericType)
            {
                elementType = elementType.GetGenericArguments()[0];
            }
            var collectionName = elementType.Name;
            var ps = PluralizationService.CreateService(System.Threading.Thread.CurrentThread.CurrentUICulture);
            if (ps.IsSingular(collectionName))
            {
                collectionName = ps.Pluralize(collectionName);
            }
            return collectionName;
        }
    }
}
