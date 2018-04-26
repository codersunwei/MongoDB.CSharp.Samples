using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Chris.Framework.Data.MongoDb
{
    public interface IMongoRepository<T> where T : IEntity
    {
        #region Mongo Specific
        IMongoCollection<T> Collection { get; }

        FilterDefinitionBuilder<T> Filter { get; }

        ProjectionDefinitionBuilder<T> Projection { get; }

        UpdateDefinitionBuilder<T> Updater { get; }
        #endregion

        #region CRUD

        #region Create

        #region Insert

        void Insert(T entity);

        Task InsertAsync(T entity);

        void Insert(IEnumerable<T> entities);

        Task InsertAsync(IEnumerable<T> entities);

        #endregion

        #endregion

        #region Retrieve

        #region Find

        IEnumerable<T> Find(Expression<Func<T, bool>> filter);

        IEnumerable<T> Find(Expression<Func<T, bool>> filter, int pageIndex, int pageSize);

        IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int pageSize);

        IEnumerable<T> Find(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, int pageIndex, int pageSize, bool isDescending);
        #endregion

        #region FindAll

        IEnumerable<T> FindAll();
        IEnumerable<T> FindAll(int pageIndex, int pageSize);
        IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int pageSize);
        IEnumerable<T> FindAll(Expression<Func<T, object>> order, int pageIndex, int pageSize, bool isDescending);

        #endregion

        #region First

        T First();

        T First(Expression<Func<T, bool>> filter);

        T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order);

        T First(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending);

        #endregion

        #region Last

        T Last();

        T Last(Expression<Func<T, bool>> filter);

        T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order);

        T Last(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order, bool isDescending);
        #endregion

        #region Get

        T Get(string id);

        #endregion

        #endregion

        #region Update

        #region Replace

        bool Replace(T entity);

        Task<bool> ReplaceAsync(T entity);

        void Replace(IEnumerable<T> entities);

        #endregion

        bool Update(string id, params UpdateDefinition<T>[] updates);

        Task<bool> UpdateAsync(string id, params UpdateDefinition<T>[] updates);

        bool Update(T entity, params UpdateDefinition<T>[] updates);

        Task<bool> UpdateAsync(T entity, params UpdateDefinition<T>[] updates);

        bool Update(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        Task<bool> UpdateAsync(FilterDefinition<T> filter, params UpdateDefinition<T>[] updates);

        bool Update(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);

        Task<bool> UpdateAsync(Expression<Func<T, bool>> filter, params UpdateDefinition<T>[] updates);

        bool Update<TField>(T entity, Expression<Func<T, TField>> field, TField value);

        Task<bool> UpdateAsync<TField>(T entity, Expression<Func<T, TField>> field, TField value);

        bool Update<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);

        Task<bool> UpdateAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value);

        #endregion

        #region Delete

        /// <summary>
        /// 根据Id删除一条记录
        /// </summary>
        /// <param name="id">string 类型的ObjectId</param>
        /// <returns>操作结果</returns>
        bool Delete(string id);

        /// <summary>
        /// 根据Id删除一条记录
        /// </summary>
        /// <param name="id">string 类型的ObjectId</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// 根据实体中的Id删除一条记录
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>操作结果</returns>
        bool Delete(T entity);

        /// <summary>
        /// 根据实体中的Id删除一条记录
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// 根据筛选条件删除一条或多条记录
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns>操作结果</returns>
        bool Delete(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 根据筛选条件删除一条或多条记录
        /// </summary>
        /// <param name="filter">筛选条件</param>
        /// <returns>操作结果</returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> filter);

        /// <summary>
        /// 删除当前集合的所有记录
        /// </summary>
        /// <returns>操作结果</returns>
        bool DeleteAll();

        /// <summary>
        /// 删除当前集合的所有记录
        /// </summary>
        /// <returns>操作结果</returns>
        Task<bool> DeleteAllAsync();
        #endregion

        #region Utils

        bool Any(Expression<Func<T, bool>> filter);

        #region Count

        long Count();

        Task<long> CountAsync();

        long Count(Expression<Func<T, bool>> filter);

        Task<long> CountAsync(Expression<Func<T, bool>> filter);

        #endregion

        #endregion

        #endregion

    }
}
