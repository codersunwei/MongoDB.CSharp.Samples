using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chris.Framework.Data.MongoDb
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public class Entity : IEntity
    {
        private DateTime _createdOn;
        private DateTime _modifiedOn;

        public Entity()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        /// <inheritdoc />
        /// <summary>
        /// string 类型的ObjectId。
        /// <remarks>自动生成Id，不用手动赋值。</remarks>
        /// </summary>
        [BsonElement(Order = 0)]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonElement("_c", Order = 1)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn
        {
            get
            {
                if (_createdOn == DateTime.MinValue)
                {
                    _createdOn = ObjectId.CreationTime;
                }

                return _createdOn;
            }
            set => _createdOn = value;
        }

        /// <inheritdoc />
        /// <summary>
        /// 修改时间
        /// </summary>
        [BsonElement("_m", Order = 2)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ModifiedOn
        {
            get
            {
                if (_modifiedOn == DateTime.MinValue)
                {
                    _modifiedOn = ObjectId.CreationTime;
                }

                return _modifiedOn;
            }
            set => _modifiedOn = value;
        }

        /// <inheritdoc />
        /// <summary>
        /// ObjectId 类型的Id
        /// </summary>
        public ObjectId ObjectId => ObjectId.Parse(Id);
    }

    public class Entity<T> : Entity
    {
        public T Content { get; set; }
    }
}
