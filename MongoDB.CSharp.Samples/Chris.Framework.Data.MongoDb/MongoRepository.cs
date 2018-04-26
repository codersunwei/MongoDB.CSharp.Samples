using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using Polly;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Chris.Framework.Data.MongoDb
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity
    {


        #region Mongo Specific
        public MongoRepository(IConfiguration config)
        {
            Collection = Database<T>.GetCollection(config);

        }

        public MongoRepository(string connectionString)
        {
            Collection = Database<T>.GetCollectionFromConnectionString(connectionString);

        }

        public MongoRepository(string connectionString, string collectionName)
        {
            Collection = Database<T>.GetCollectionFromConnectionString(connectionString, collectionName);

        }

        public IMongoCollection<T> Collection { get; }


        public FilterDefinitionBuilder<T> Filter => Builders<T>.Filter;

        public ProjectionDefinitionBuilder<T> Projection => Builders<T>.Projection;

        public UpdateDefinitionBuilder<T> Updater => Builders<T>.Update;

        public IFindFluent<T, T> Query() => Collection.Find(Filter.Empty);

        public IFindFluent<T, T> Query(Expression<Func<T, bool>> filter) => Collection.Find(filter);

        #endregion

        #region Utils

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return Retry(() => First(filter) != null);
        }

        public long Count()
        {
            return Retry(() => Collection.Count(Filter.Empty));
        }

        public long Count(Expression<Func<T, bool>> filter)
        {
            return Retry(() => Collection.Count(filter));
        }

        public Task<long> CountAsync()
        {
            return Retry(() => Collection.CountAsync(Filter.Empty));
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> filter)
        {
            return Retry(() => Collection.CountAsync(filter));
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        /// <summary>
        /// 根据Id删除一条记录
        /// </summary>
        /// <param name="id">string 类型的ObjectId</param>
        /// <returns></returns>
        public virtual bool Delete(string id)
        {
            return Retry(() => { return Collection.DeleteOne(i => i.Id == id).IsAcknowledged; });
        }

        /// <inheritdoc />
        /// <summary>
        /// 根据实体中的Id删除一条记录
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            return Delete(entity.Id);
        }

        /// <inheritdoc />
        /// <summary>
        /// 根据筛选条件删除一条或多条记录
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> filter)
        {
            return Retry(() => Collection.DeleteMany(filter).IsAcknowledged);
        }

        /// <inheritdoc />
        /// <summary>
        /// 删除当前集合的所有记录
        /// </summary>
        /// <returns></returns>
        public virtual bool DeleteAll()
        {
            return Retry(() => Collection.DeleteMany(Filter.Empty).IsAcknowledged);
        }

        /// <inheritdoc />
        /// <summary>
        /// 删除当前集合的所有记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAllAsync()
        {
            return await Retry(async () =>
            {
                var deleteResult = await Collection.DeleteManyAsync(Filter.Empty);

                return deleteResult.IsAcknowledged;
            });
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            return await Retry(async () =>
            {
                var deleteResult = await Collection.DeleteOneAsync(i => i.Id == id);

                return deleteResult.IsAcknowledged;

            });

        }

        public async Task<bool> DeleteAsync(T entity)
        {
            return await DeleteAsync(entity.Id);
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return await Retry(async () =>
            {
                var deleteResult = await Collection.DeleteManyAsync(filter);

                return deleteResult.IsAcknowledged;
            });
        }

        #endregion

        #region Find && FindAll
        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return Query(filter).ToEnumerable();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int pageSize)
        {
            return Find(filter, i => i.Id, pageIndex, pageSize);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int pageSize)
        {
            return Find(filter, order, pageIndex, pageSize, true);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int pageSize, bool isDescending)
        {
            return Retry(() =>
            {
                var query = Query(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize);

                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        public virtual IEnumerable<T> FindAll()
        {
            return Retry(() => Query().ToEnumerable());
        }

        public IEnumerable<T> FindAll(int pageIndex, int pageSize)
        {
            return FindAll(i => i.Id, pageIndex, pageSize);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int pageSize)
        {
            return FindAll(order, pageIndex, pageSize, true);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int pageSize, bool isDescending)
        {
            return Retry(() =>
            {
                var query = Query().Skip(pageIndex * pageSize).Limit(pageSize);

                return (isDescending ? query.SortByDescending(order) : query.SortBy(order)).ToEnumerable();
            });
        }

        #endregion

        #region First

        public T First()
        {
            return FindAll(i => i.Id, 0, 1, false).FirstOrDefault();
        }

        public T First(Expression<Func<T, bool>> filter)
        {
            return First(filter, i => i.Id);
        }

        public T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return First(filter, order, false);
        }

        public T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return Find(filter, order, 1, 1, isDescending).FirstOrDefault();
        }

        #endregion

        #region Get
        public T Get(string id)
        {
            return Retry(() => Find(i => i.Id == id).FirstOrDefault());
        }
        public Task<T> GetAsync(string id)
        {
            return Task.Run(() => Get(id));
        }

        #endregion

        #region Insert

        public virtual void Insert(T entity)
        {
            Retry(() =>
             {
                 Collection.InsertOne(entity);

                 return true;
             });
        }

        public void Insert(IEnumerable<T> entities)
        {
            Retry(() =>
            {
                Collection.InsertMany(entities);
                return true;
            });
        }

        public Task InsertAsync(T entity)
        {
            return Retry(() => Collection.InsertOneAsync(entity));
        }

        public Task InsertAsync(IEnumerable<T> entities)
        {
            return Retry(() => Collection.InsertManyAsync(entities));
        }

        #endregion

        #region Last

        public T Last()
        {
            return FindAll(i => i.Id, 0, 1, true).FirstOrDefault();
        }

        public T Last(Expression<Func<T, bool>> filter)
        {
            return Last(filter, i => i.Id);
        }

        public T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order)
        {
            return Last(filter, order, false);
        }

        public T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending)
        {
            return First(filter, order, !isDescending);
        }

        #endregion

        #region Replace

        public bool Replace(T entity)
        {
            return Retry(() => Collection.ReplaceOne(i => i.Id == entity.Id, entity).IsAcknowledged);
        }

        public void Replace(IEnumerable<T> entities)
        {

            foreach (var entity in entities)
            {
                Replace(entity);
            }


        }

        public Task<bool> ReplaceAsync(T entity)
        {
            return Retry(() =>
            {
                return Task.Run(() => Replace(entity));

            });
        }

        #endregion

        #region Update

        public bool Update(string id, params UpdateDefinition<T>[] updates)
        {
            return Update(Filter.Eq(i => i.Id, id), updates);
        }

        public bool Update(T entity, params UpdateDefinition<T>[] updates)
        {
            return Update(entity.Id, updates);
        }

        public bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedOn);
                return Collection.UpdateMany(filter, update.CurrentDate(i => i.ModifiedOn)).IsAcknowledged;
            });
        }

        public bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                var update = Updater.Combine(updates).CurrentDate(i => i.ModifiedOn);
                return Collection.UpdateMany(filter, update).IsAcknowledged;
            });
        }

        public bool Update<TField>(T entity, Expression<Func<T, TField>> field, TField value)
        {
            return Update(entity, Updater.Set(field, value));
        }

        public bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return Update(filter, Updater.Set(field, value));
        }

        public Task<bool> UpdateAsync(string id, params UpdateDefinition<T>[] updates)
        {
            return Task.Run(() => Update(Filter.Eq(i => i.Id, id), updates));
        }

        public Task<bool> UpdateAsync(T entity, params UpdateDefinition<T>[] updates)
        {
            return Task.Run(() => Update(entity.Id, updates));
        }

        public Task<bool> UpdateAsync(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                return Task.Run(() => Update(filter, updates));
            });
        }

        public Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates)
        {
            return Retry(() =>
            {
                return Task.Run(() => Update(filter, updates));
            });
        }

        public Task<bool> UpdateAsync<TField>(T entity, Expression<Func<T, TField>> field, TField value)
        {
            return Task.Run(() => Update(entity, Updater.Set(field, value)));
        }

        public Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value)
        {
            return Task.Run(() => Update(filter, Updater.Set(field, value)));
        }

        #endregion

        protected virtual TResult Retry<TResult>(Func<TResult> action)
        {
            return Policy
                .Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) ||
                                                       i.InnerException.GetType() == typeof(SocketException))
                .Retry(3)
                .Execute(action);
        }
    }
}
