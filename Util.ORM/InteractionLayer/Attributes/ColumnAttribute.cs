using System;
using System.Data;

namespace Util.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]   
    public sealed class ColumnAttribute : Attribute
    {
        public ColumnAttribute() { }
        public ColumnAttribute(string name) => Name = name;
        /// <summary>
        /// 对应数据库表里的字段名称
        /// </summary>
        public string? Name { get; set; }
        private bool _isPrimaryKey;
        /// <summary>
        /// 是否是主键
        /// <para>默认false</para>
        /// </summary>
        public bool IsPrimaryKey { get => _isPrimaryKey || IsAutoIncrement; set => _isPrimaryKey = value; }
        /// <summary>
        /// 是否是int自增列
        /// <para>默认false,如过设置为true,IsPrimaryKey也会为true</para>
        /// </summary>
        public bool IsAutoIncrement { get; set; }
        /// <summary>
        /// 对应数据库表里的字段类型
        /// <para>此字段暂未实现功能</para>
        /// </summary>
        public DbType? DbType { get; set; }
    }
}
