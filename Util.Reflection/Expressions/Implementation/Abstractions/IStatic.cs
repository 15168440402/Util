using System;

namespace Util.Reflection.Expressions.Abstractions
{
    /// <summary>
    /// 标记为静态类
    /// <para>拥有静态类的（属性、方法）等功能</para>
    /// <para>未拥有给属性赋值的功能</para>
    /// </summary>
    public interface IStatic
    {
        public Type Type {get;set;}
    }
}
