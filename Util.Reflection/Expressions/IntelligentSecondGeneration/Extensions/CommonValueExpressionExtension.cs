
using Util.Reflection.Expressions.IntelligentGeneration.Extensions;

namespace Util.Reflection.Expressions.Abstractions
{
    public abstract partial class CommonValueExpression
    {
        //public static implicit operator CommonValueExpression(Var value) => value.Variable;

        public static implicit operator CommonValueExpression(string value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(long value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(ulong value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(int value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(uint value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(short value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(ushort value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(byte value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(sbyte value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(nint value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(nuint value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(float value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(double value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(decimal value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(bool value) => Expr.Constant(value);

        public static implicit operator CommonValueExpression(char value) => Expr.Constant(value);

        /// <summary>
        /// 获取属性\设置属性
        /// <para>注意：获取属性有可能会获取到空参数的方法</para>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public CommonValueExpression this[string memberName]
        {
            get
            {
                return this.MemberQuery(memberName);
            }
        }
        /// <summary>
        /// 索引器获取\赋值
        /// </summary>
        /// <param name="propertyIndexParams"></param>
        /// <returns></returns>
        public CommonValueExpression this[params object[] propertyIndexParams]
        {
            get
            {
                return this.MemberQuery(propertyIndexParams.ToExpr());
            }
        }
    }
}
