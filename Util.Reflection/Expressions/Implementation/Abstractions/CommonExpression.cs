using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Util.Reflection.Expressions.Util;

namespace Util.Reflection.Expressions.Abstractions
{
    /// <summary>
    /// 通用表达式
    /// <para>用于转化为 <see cref="Expression" /></para>
    /// <para>用于生成委托 <see cref="Delegate" /></para>
    /// </summary>
    public abstract class CommonExpression : IDelegteBuilder
    {
        /// <summary>
        /// 对应的名称
        /// <para>根据场景会作用于变量名称、属性名称、方法名称、参数名称</para>
        /// </summary>
        public string Name { get; set; } = "";
        [DisallowNull]
        private Type? _type;
        /// <summary>
        /// 对应的数据类型
        /// </summary>
        [DisallowNull]
        //[NotNull]
        public virtual Type? Type { get => _type ?? throw new Exception("类型Type不可为null"); set => _type = value; }
        /// <summary>
        /// 表达式节点
        /// <para>用于方便查看当前表达式的作用类型</para>
        /// </summary>
        public abstract ExprType NodeType { get; protected set; }      
        
        protected Expression? _exp;
        /// <summary>
        /// 获取CommonExpression转换的 <see cref="Expression" />
        /// <para>多次获取只会转换一次</para>
        /// </summary>
        /// <returns></returns>
        public virtual Expression GetExpression()
        {
            return _exp ?? (_exp = ConvertToExpression());
        }
        /// <summary>
        /// CommonExpression转换为 <see cref="Expression" />
        /// </summary>
        /// <returns></returns>
        private protected abstract Expression ConvertToExpression();
        /// <summary>
        /// CommonExpression隐式转换为 <see cref="Expression" />
        /// </summary>
        /// <param name="customExp"></param>
        public static implicit operator Expression(CommonExpression customExp)
        {
            return customExp.GetExpression();
        }
        /// <summary>
        /// 生成委托 <see cref="Delegate" />
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <returns></returns>
        public virtual TDelegate BuildDelegate<TDelegate>() where TDelegate : Delegate
        {
            return ExpressionUtil.BuildDelegate<TDelegate>(GetExpression());
        }
        /// <summary>
        /// 返回委托<see cref="Delegate" />的表达式内容视图
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var property = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
            var debugView = property.GetValue(GetExpression())?.ToString()??"";
            return debugView;
        }
        /// <summary>
        /// 委托<see cref="Delegate" />的表达式内容视图
        /// </summary>
        public string ExpView => ToString();
    }
}
