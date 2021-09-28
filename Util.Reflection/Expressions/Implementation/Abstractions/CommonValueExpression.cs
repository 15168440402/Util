using System;
using System.Diagnostics.CodeAnalysis;

namespace Util.Reflection.Expressions.Abstractions
{
    /// <summary>
    /// 标识为值表达式
    /// <para>拥有属性、方法、作为运算符使用的参数等功能</para>
    /// <para>未拥有给属性赋值的功能</para>
    /// <para>注解：值代表class/struct实例化后的数据</para>
    /// </summary>
    public abstract partial class CommonValueExpression : CommonExpression
    {
        [DisallowNull]
        private Type? _type;

        /// <summary>
        /// 值的类型<see cref="System.Type" />
        /// </summary>
        [DisallowNull]
        public override Type Type { get => (_type == typeof(void) || _type is null ? throw new UtilException("返回Type为void/null，无法进行成员操作", "检查是否对返参为void的方法返回值进行了成员操作") : _type); set => _type = value; }

        public CommonValueExpression(Type type) => Type = type;
    }
}
