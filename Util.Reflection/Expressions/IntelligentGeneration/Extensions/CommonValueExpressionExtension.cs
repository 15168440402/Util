using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Reflection.Expressions.Abstractions
{
    public abstract partial class CommonValueExpression
    {
        /// <summary>
        /// 前置--
        /// <para>--x</para>
        /// </summary>
        /// <returns></returns>
        public CommonValueExpression PreDecrementAssign()
        {
            return OperationExpression.PreDecrementAssign(this);
        }
        /// <summary>
        /// 后置--
        /// <para>x--</para>
        /// </summary>
        /// <returns></returns>
        public CommonValueExpression PostDecrementAssign()
        {
            return OperationExpression.PostDecrementAssign(this);
        }
        /// <summary>
        /// 前置++
        /// <para>++x</para>
        /// </summary>
        /// <returns></returns>
        public CommonValueExpression PreIncrementAssign()
        {
            return OperationExpression.PreIncrementAssign(this);
        }
        /// <summary>
        /// 后置++
        /// <para>x++</para>
        /// </summary>
        /// <returns></returns>
        public CommonValueExpression PostIncrementAssign()
        {
            return OperationExpression.PostIncrementAssign(this);
        }
        public static CommonValueExpression operator --(CommonValueExpression value)
        {
            return value.PreDecrementAssign();
        }
        public static CommonValueExpression operator ++(CommonValueExpression value)
        {
            return value.PreIncrementAssign();
        }
        public static CommonValueExpression operator +(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.Add(left, right);
        }
        public static CommonValueExpression operator -(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.Subtract(left, right);
        }
        public static CommonValueExpression operator *(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.Multiply(left, right);
        }
        public static CommonValueExpression operator /(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.Divide(left, right);
        }
        public static CommonValueExpression operator +(CommonValueExpression left)
        {
            return OperationExpression.UnaryPlus(left);
        }
        public static CommonValueExpression operator -(CommonValueExpression left)
        {
            return OperationExpression.Negate(left);
        }
        public static CommonValueExpression operator !=(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.NotEqual(left, right);
        }
        public static CommonValueExpression operator ==(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.Equal(left, right);
        }
        public static CommonValueExpression operator <(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.LessThan(left, right);
        }
        public static CommonValueExpression operator >(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.GreaterThan(left, right);
        }
        public static CommonValueExpression operator <=(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.LessThanOrEqual(left, right);
        }
        public static CommonValueExpression operator >=(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.GreaterThanOrEqual(left, right);
        }
        public static CommonValueExpression operator &(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.AndAlso(left, right);
        }
        public static CommonValueExpression operator |(CommonValueExpression left, CommonValueExpression right)
        {
            return OperationExpression.OrElse(left, right);
        }
        public static bool operator true(CommonValueExpression left)
        {
            return false;
        }
        public static bool operator false(CommonValueExpression left)
        {
            return false;
        }
    }
}
