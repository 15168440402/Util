using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Reflection.Expressions
{
    public partial class Expr
    {
        public static ParamExpression Param(Type type, string name = "") => new ParamExpression(type, name);
        public static ParamExpression Param<T>(string name = "") => Param(typeof(T), name);
        public static VariableExpression Variable(Type type, string name = "") => new VariableExpression(type, name);
        public static VariableExpression Variable<T>(string name = "") => Variable(typeof(T), name);
        public static ConstantExpression Constant(object value, string name = "") => new ConstantExpression(value, value.GetType(), name);
        public static ConstantExpression Constant(object? value, Type type, string name = "") => new ConstantExpression(value, type, name);
        public static ConstantExpression Constant<T>(T? value, string name = "") => new ConstantExpression(value, typeof(T), name);       
        public static Static Static(Type type) => new Static(type);
        public static Static Static<T>() => Static(typeof(T));
        public static InstanceExpression New(Type type, IEnumerable<CommonValueExpression> constructorParams)
        {
            return new InstanceExpression(type, constructorParams);
        }
        public static InstanceExpression New<T>(IEnumerable<CommonValueExpression> constructorParams)
        {
            return New(typeof(T), constructorParams);
        }
        public static InstanceExpression New(Type type, params CommonValueExpression[] constructorParams)
        {
            return New(type, constructorParams as IEnumerable<CommonValueExpression>);
        }
        public static InstanceExpression New<T>(params CommonValueExpression[] constructorParams)
        {
            return New<T>(constructorParams as IEnumerable<CommonValueExpression>);
        }
        public static BodyExpression Block(IEnumerable<CommonVariableExpression>? variables, IEnumerable<CommonExpression> units, CommonValueExpression? returnExp = null)
        {
            return new BodyExpression(variables, units, returnExp);
        }
        public static BodyExpression Block(CommonVariableExpression variable, IEnumerable<CommonExpression> units, CommonValueExpression? returnExp = null)
        {
            return Block(new[] { variable }, units, returnExp);
        }
        public static BodyExpression Block(IEnumerable<CommonExpression> units, CommonValueExpression? returnExp = null)
        {
            return Block(variables: null, units, returnExp);
        }
        public static OperationExpression Assign(CommonVariableExpression left, CommonValueExpression right) => OperationExpression.Assign(left, right);
        public static ConditionalExpression IfThen(CommonValueExpression conditional, CommonExpression ifTrue)
        {
            return new ConditionalExpression(conditional, ifTrue, null);
        }
        public static ConditionalExpression IfThenElse(CommonValueExpression conditional, CommonExpression ifTrue, CommonExpression ifFalse)
        {
            return new ConditionalExpression(conditional, ifTrue, ifFalse);
        }
        public static LoopExpression Loop(CommonExpression body, LabelTarget breakTarget, LabelTarget continueTarget)
        {
            return new LoopExpression(body, breakTarget, continueTarget);
        }
        public static ReturnExpression Return(LabelTarget target) => new ReturnExpression(target);
        public static ContinueExpression Continue(LabelTarget target) => new ContinueExpression(target);
        public static BreakExpression Break(LabelTarget target) => new BreakExpression(target);
    }
    public partial class Expr<T>
    {
        public static ConstantExpression Constant(T? value, string name = "") => Expr.Constant<T>(value, name);
        public static ParamExpression Param => Expr.Param<T>();
        public static InstanceExpression New(IEnumerable<CommonValueExpression> constructorParams) => Expr.New<T>(constructorParams);
        public static InstanceExpression New(params CommonValueExpression[] constructorParams) => Expr.New<T>(constructorParams);
        public static Static Static => Expr.Static<T>();
    }
}
