using System.Reflection;
using Chris.Framework.Data.MongoDb.Attributes;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Chris.Framework.Data.MongoDb
{
    internal class Database<T> where T : IEntity
    {
        private Database()
        {

        }

        internal static IMongoCollection<T> GetCollection(IConfiguration config)
        {
            return GetCollectionFromConnectionString(GetDefaultConnectionString(config));
        }

        internal static IMongoCollection<T> GetCollectionFromConnectionString(string connectionString)
        {
            return GetCollectionFromConnectionString(connectionString, GetCollectionName());
        }

        internal static IMongoCollection<T> GetCollectionFromConnectionString(string connectionString, string collectionName)
        {
            return GetCollectionFromUrl(new MongoUrl(connectionString), collectionName);
        }

        internal static IMongoCollection<T> GetCollectionFromUrl(MongoUrl url)
        {
            return GetCollectionFromUrl(url, GetCollectionName());
        }

        internal static IMongoCollection<T> GetCollectionFromUrl(MongoUrl url, string collectionName)
        {
            return GetDatabaseFromUrl(url).GetCollection<T>(collectionName);
        }


        private static IMongoDatabase GetDatabaseFromUrl(MongoUrl url)
        {
            var client = new MongoClient();

            return client.GetDatabase(url.DatabaseName);

        }

        #region Collection Name

        private static string GetCollectionName()
        {
            var collectionName = typeof(T).GetTypeInfo().BaseType == typeof(object)
                ? GetCollectionNameFromInterface
                : GetCollectionNameFromType;

            if (string.IsNullOrEmpty(collectionName))
            {
                collectionName = typeof(T).Name;
            }

            return collectionName.ToLowerInvariant();
        }

        private static string GetCollectionNameFromInterface
        {
            get
            {
                var attr = typeof(T).Assembly.GetCustomAttribute<CollectionNameAttribute>();
                return attr?.Name ?? typeof(T).Name;
            }

        }

        private static string GetCollectionNameFromType
        {
            get
            {
                var entityType = typeof(T);

                var attr = typeof(T).Assembly.GetCustomAttribute<CollectionNameAttribute>();

                var collectionName = attr != null ? attr.Name : entityType.Name;

                return collectionName;
            }

        }

        #endregion

        #region Connection Name

        private static string GetConnectionName()
        {
            var connectionName = typeof(T).GetTypeInfo().BaseType == typeof(object) ?
                GetConnectionNameFromInterface :
                ConnectionNameFromType;

            if (string.IsNullOrEmpty(connectionName))
            {
                connectionName = typeof(T).Name;
            }
            return connectionName.ToLowerInvariant();
        }

        private static string GetConnectionNameFromInterface
        {
            get
            {
                var att = typeof(T).GetTypeInfo().Assembly.GetCustomAttribute<ConnectionNameAttribute>();

                return att?.Name ?? typeof(T).Name;
            }

        }

        private static string ConnectionNameFromType
        {
            get
            {
                var entitytype = typeof(T);

                string collectionname;

                var att = typeof(T).GetTypeInfo().Assembly.GetCustomAttribute<ConnectionNameAttribute>();

                if (att != null)
                {
                    // It does! Return the value specified by the ConnectionName attribute
                    collectionname = att.Name;
                }
                else
                {
                    if (typeof(Entity).GetTypeInfo().IsAssignableFrom(entitytype))
                    {
                        // No attribute found, get the basetype
                        while (entitytype.GetTypeInfo().BaseType != typeof(Entity))
                        {
                            entitytype = entitytype.GetTypeInfo().BaseType;
                        }
                    }
                    collectionname = entitytype.Name;
                }

                return collectionname;
            }
        }

        #endregion

        #region ConnectionString
        /// <summary>
        /// 从app.config 或 Web.config 文件查找ConnectionStrings节点的默认配置
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static string GetDefaultConnectionString(IConfiguration config)
        {
            return config.GetConnectionString(GetConnectionName());
        }
        #endregion
    }
}
