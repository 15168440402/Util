using System;
using System.Linq.Expressions;

namespace Util.Reflection.Expressions.Abstractions
{
    /// <summary>
    /// 标识为变量表达式
    /// <para>拥有属性、方法、作为运算符使用的参数等功能</para>
    /// <para>拥有为属性赋值功能</para>
    /// <para>注解：变量代表c#语言中的变量，值一般赋值/绑定给变量</para>
    /// </summary>
    public abstract class CommonVariableExpression : CommonValueExpression
    {
        public CommonVariableExpression(Type type) : base(type)
        {

        }
        public override ParameterExpression GetExpression()
        {
            return (ParameterExpression)base.GetExpression();
        }
        private protected abstract override ParameterExpression ConvertToExpression();
    }
}
