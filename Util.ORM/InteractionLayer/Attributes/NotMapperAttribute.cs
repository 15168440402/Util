using System;

namespace Util.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotMapperAttribute : Attribute
    {
        public NotMapperAttribute(NotMapper flag=NotMapper.WhenInsert|NotMapper.WhenUpdate|NotMapper.WhenSelect)
        {
            (WhenInsert, WhenUpdate, WhenSelect) = flag switch
            {
                NotMapper.WhenInsert | NotMapper.WhenUpdate | NotMapper.WhenSelect => (true, true, true),
                NotMapper.WhenInsert => (true, false, false),
                NotMapper.WhenUpdate => (false, true, false),
                NotMapper.WhenSelect => (false, false, true),
                NotMapper.WhenInsert | NotMapper.WhenUpdate => (true, true, false),
                NotMapper.WhenInsert | NotMapper.WhenSelect => (true, false, true),
                NotMapper.WhenUpdate | NotMapper.WhenSelect => (false, true, true),
                _=>(true, true, true)
            };
        }
        public bool WhenSelect { get; }
        public bool WhenUpdate { get; }
        public bool WhenInsert { get; }
    }
    [Flags]
    public enum NotMapper
    {
        WhenInsert = 1,
        WhenUpdate = 2,
        WhenSelect = 4
    }
}
