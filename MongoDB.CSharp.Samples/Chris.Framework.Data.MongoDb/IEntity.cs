using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chris.Framework.Data.MongoDb
{
    /// <summary>
    /// Mongo 数据实体 接口
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// string 类型的objectId
        /// </summary>
        [BsonId]
        string Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreatedOn { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime ModifiedOn { get; set; }
        /// <summary>
        /// objectId 类型的Id
        /// </summary>
        [BsonIgnore]
        ObjectId ObjectId { get; }
    }
}
