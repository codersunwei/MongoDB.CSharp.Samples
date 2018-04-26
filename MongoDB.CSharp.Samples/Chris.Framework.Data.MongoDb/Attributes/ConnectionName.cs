using System;

namespace Chris.Framework.Data.MongoDb.Attributes
{

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ConnectionNameAttribute : Attribute
    {

        public ConnectionNameAttribute(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Empty connection name is not allowed", nameof(value));
            }

            Name = value;
        }
        /// <summary>
        /// 获取连接的名称
        /// </summary>
        public virtual string Name { get; }
    }
}
