using System;
using System.Collections.Generic;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Reflection.Expressions
{
    public static class ExpressionExtension
    {
        public static MemberQueryExpression MemberQuery<T>(this T expr, IEnumerable<CommonValueExpression> propertyIndexParams) where T : CommonValueExpression,IDelegteBuilder
        {
            return MemberQueryExpression.MemberQuery(expr, propertyIndexParams);
        }
        public static MemberQueryExpression MemberQuery<T>(this T expr, string memberName, IEnumerable<Type>? methodParamTypes=null) where T : CommonValueExpression, IDelegteBuilder
        {
            return MemberQueryExpression.MemberQuery(expr, memberName, methodParamTypes);
        }
        public static MemberBindExpression MemberBind<T>(this T expr, CommonValueExpression member, IEnumerable<CommonValueExpression>? propertyIndexParams=null) where T : CommonVariableExpression, IDelegteBuilder
        {
            return MemberBindExpression.MemberBind(expr, member, propertyIndexParams);
        }
        public static MethodExpression Method<T>(this T expr, string methodName, IEnumerable<CommonValueExpression>? param=null) where T : CommonValueExpression, IDelegteBuilder
        {
            return MethodExpression.Method(expr, methodName, param);
        }
        public static MethodExpression Method<T>(this T expr, string methodName, params CommonValueExpression[] param) where T : CommonValueExpression, IDelegteBuilder
        {
            return MethodExpression.Method(expr, methodName, param);
        }
        public static ConvertExpression Convert<T>(this T expr, Type converType) where T : CommonValueExpression, IDelegteBuilder
        {
            return new ConvertExpression(expr, converType);
        }
        public static ConvertExpression Convert<ConvertT>(this CommonValueExpression expr)
        {
            return new ConvertExpression(expr, typeof(ConvertT));
        }

        public static MemberQueryExpression MemberQuery(this IStatic @static, string memberName, IEnumerable<Type>? methodParamTypes = null)
        {
            return MemberQueryExpression.MemberQuery(@static.Type, memberName, methodParamTypes);
        }
        public static MemberBindExpression MemberBind(this IStatic @static, CommonValueExpression member)
        {
            return MemberBindExpression.MemberBind(@static.Type, member);
        }
        public static MethodExpression Method(this IStatic @static, string methodName, IEnumerable<CommonValueExpression>? param = null)
        {
            return MethodExpression.Method(@static.Type, methodName, param);
        }
        public static MethodExpression Method(this IStatic @static, string methodName, params CommonValueExpression[] param)
        {
            return MethodExpression.Method(@static.Type, methodName, param);
        }
    }
}
