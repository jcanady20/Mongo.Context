using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mongo.Context.Mapping;

namespace Mongo.Context
{
    public class MongoBuilder : IMongoBuilder
    {
        private Type _contextType;
        private MongoContext _context;
        private readonly static IDictionary<Type, MongoClassMap> _typeClassMaps = new Dictionary<Type, MongoClassMap>();
        public MongoBuilder(MongoContext context)
        {
            _contextType = context.GetType();
            _context = context;
        }

        public bool IsFrozen
        {
            get
            {
                return _typeClassMaps.Values.Any(x => x.IsFrozen);
            }
        }

        public void InitializeSets()
        {
            var setFinder = new Internal.MongoSetFinder();
            var setSource = new Internal.MongoSetSource();
            var setInitializer = new Internal.MongoSetInitializer(setFinder, setSource);
            setInitializer.InitializeSets(_context, _typeClassMaps);
        }

        public void InitializeIndexes()
        {
            var setFinder = new Internal.MongoSetFinder();
            foreach (var setinfo in setFinder.FindSets(_context))
            {
                var mcm = _typeClassMaps[setinfo.EntityType];
                if(mcm == null)
                {
                    continue;
                }
                _context.EnsureIndexes(mcm.CollectionName, mcm.Indexes);
            }
        }

        public MongoClassMap<T> Entry<T>()
        {
            return (MongoClassMap<T>)_typeClassMaps[typeof(T)];
        }

        public void FromAssembly(Assembly assembly)
        {
            var classMaps = assembly.GetTypes().Where(x => x.IsClass && x.IsSubclassOf(typeof(MongoClassMap)));
            foreach (var cm in classMaps)
            {
                var baseType = cm.BaseType;
                //  Check to see if this type has already been registred
                if (baseType.IsGenericType)
                {
                    var AssignableType = baseType.GetGenericArguments().First();
                    if (MongoClassMap.IsClassMapRegistered(AssignableType))
                    {
                        continue;
                    }
                }
                var instance = (MongoClassMap)Activator.CreateInstance(cm);
                MongoClassMap.RegisterClassMap(instance);
                _typeClassMaps.Add(instance.ClassType, instance);
            }
        }

        public static MongoClassMap LookupClassMap(Type type)
        {
            return _typeClassMaps[type];
        }
    }
}
