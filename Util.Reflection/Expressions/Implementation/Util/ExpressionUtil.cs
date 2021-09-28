using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Util.Reflection.Expressions.Util
{
    public static class ExpressionUtil
    {
        public static TDelegate BuildDelegate<TDelegate>(Expression exp, List<ParameterExpression>? parameters = null) where TDelegate : Delegate
        {
            var paramters = GetParamterExpr(exp);
            return Expression.Lambda<TDelegate>(exp, paramters).Compile();
        }
        static List<ParameterExpression> GetParamterExpr(Expression? exp)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();
            if (exp is ParameterExpression parameterExp && parameterExp.Name!=null) parameters.Add(parameterExp);
            if (exp is MemberInitExpression memberInitExp)
            {
                foreach (var memberExp in memberInitExp.Bindings.Select(b => ((MemberAssignment)b).Expression))
                {
                    parameters.AddRange(GetParamterExpr(memberExp));
                }
            }
            if (exp is MemberExpression)
            {
                var memberExp = (MemberExpression)exp;
                parameters.AddRange(GetParamterExpr(memberExp.Expression));
            }
            if (exp is MethodCallExpression methodCallExp)
            {
                parameters.AddRange(GetParamterExpr(methodCallExp.Object));
                foreach (var arg in methodCallExp.Arguments)
                {
                    parameters.AddRange(GetParamterExpr(arg));
                }
            }
            if (exp is BinaryExpression binaryExp)
            {
                parameters.AddRange(GetParamterExpr(binaryExp.Right));
            }
            if (exp is BlockExpression blockExp)
            {
                foreach (var itemExp in blockExp.Expressions)
                {
                    parameters.AddRange(GetParamterExpr(itemExp));
                }
            }
            if(exp is UnaryExpression unaryExp)
            {
                parameters.AddRange(GetParamterExpr(unaryExp.Operand));
            }
            return parameters;
        }
    }
}
