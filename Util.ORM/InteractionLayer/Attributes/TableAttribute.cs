using System;

namespace Util.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        public TableAttribute(string name) => Name = name;
        public string Name { get; set; }
    }
}
