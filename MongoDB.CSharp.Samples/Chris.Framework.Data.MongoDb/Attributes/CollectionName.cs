using System;

namespace Chris.Framework.Data.MongoDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CollectionNameAttribute : Attribute
    {
        public CollectionNameAttribute(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Empty collection name is not allowed", nameof(value));
            }

            Name = value;
        }
        /// <summary>
        /// 获取Collection的名称
        /// </summary>
        public virtual string Name { get; }
    }
}
