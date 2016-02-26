using System;
using MongoDB.Bson.Serialization;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;
using System.Linq;

namespace Mongo.Context.Mapping
{
    public class MongoClassMap<TEntity> : MongoClassMap
    {
        public MongoClassMap() : base(typeof(TEntity))
        { }
        public MongoClassMap(Action<BsonClassMap<TEntity>> initializer) : base(typeof(TEntity))
        { }

        // Duplicating Efforts from BsonClassMap<T>
        public new TEntity CreateInstance()
        {
            return (TEntity)base.CreateInstance();
        }

        public new MongoClassMap<TEntity> SetCollectionName(string collectionName)
        {
            _collectionName = collectionName;
            return this;
        }

        public new MongoClassMap<TEntity> AddIndex(MongoIndex index)
        {
            _indexes.Add(index);
            return this;
        }

        public BsonMemberMap GetPropertyMap<TMember>(Expression<Func<TEntity, TMember>> expression)
        {
            var memberName = GetMemberNameFromExpression(expression);
            return GetMemberMap(memberName);
        }

        public BsonMemberMap MapIdProperty<TMember>(Expression<Func<TEntity, TMember>> expression)
        {
            var propertyMap = MapProperty(expression);
            SetIdMember(propertyMap);
            return propertyMap;
        }

        public BsonMemberMap MapProperty<TMember>(Expression<Func<TEntity, TMember>> expression)
        {
            var memberInfo = GetMemberInfoFromExpression(expression);
            return MapMember(memberInfo);
        }

        public void UnmapProperty<TMember>(Expression<Func<TEntity, TMember>> expression)
        {
            var memberInfo = GetMemberInfoFromExpression(expression);
            UnmapMember(memberInfo);
        }

        private static MemberInfo GetMemberInfoFromExpression<TMember>(Expression<Func<TEntity, TMember>> memberLambda)
        {
            var body = memberLambda.Body;
            MemberExpression memberExpression;
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    memberExpression = (MemberExpression)body;
                    break;
                case ExpressionType.Convert:
                    var convertExpression = (UnaryExpression)body;
                    memberExpression = (MemberExpression)convertExpression.Operand;
                    break;
                default:
                    throw new BsonSerializationException("Invalid lambda expression");
            }
            var memberInfo = memberExpression.Member;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    break;
                case MemberTypes.Property:
                    if (memberInfo.DeclaringType.IsInterface)
                    {
                        memberInfo = FindPropertyImplementation((PropertyInfo)memberInfo, typeof(TEntity));
                    }
                    break;
                default:
                    memberInfo = null;
                    break;
            }
            if (memberInfo == null)
            {
                throw new BsonSerializationException("Invalid lambda expression");
            }
            return memberInfo;
        }

        private static PropertyInfo FindPropertyImplementation(PropertyInfo interfacePropertyInfo, Type actualType)
        {
            var interfaceType = interfacePropertyInfo.DeclaringType;

            // An interface map must be used because because there is no
            // other officially documented way to derive the explicitly
            // implemented property name.
            var interfaceMap = actualType.GetInterfaceMap(interfaceType);

            var interfacePropertyAccessors = interfacePropertyInfo.GetAccessors(true);

            var actualPropertyAccessors = interfacePropertyAccessors.Select(interfacePropertyAccessor =>
            {
                var index = Array.IndexOf<MethodInfo>(interfaceMap.InterfaceMethods, interfacePropertyAccessor);

                return interfaceMap.TargetMethods[index];
            });

            // Binding must be done by accessor methods because interface
            // maps only map accessor methods and do not map properties.
            return actualType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Single(propertyInfo =>
                {
                    // we are looking for a property that implements all the required accessors
                    var propertyAccessors = propertyInfo.GetAccessors(true);
                    return actualPropertyAccessors.All(x => propertyAccessors.Contains(x));
                });
        }

        private static string GetMemberNameFromExpression<TMember>(Expression<Func<TEntity, TMember>> expression)
        {
            return GetMemberInfoFromExpression(expression).Name;
        }

    }
}
