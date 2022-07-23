using System;
using System.Data.Entity.Design.PluralizationServices;
//  https://github.com/microsoft/referencesource/blob/5697c29004a34d80acdaf5742d7e699022c64ecd/System.Data.Entity.Design/System/Data/Entity/Design/PluralizationService/EnglishPluralizationService.cs#L20
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
