using System;
using System.Collections.Generic;
using Util.Reflection.Expressions.Abstractions;
using Util.Reflection.Expressions.IntelligentGeneration.Extensions;

namespace Util.Reflection.Expressions
{
    public partial class Var: VariableExpression
    {
        internal ExprStep Step { get; init; }
        public Var(Type type):base(type)
        {
            Step = ExprStepsContainer.QueryExprStep();
        }
        public Var(CommonValueExpression value) : this(value.Type)
        {
            Step.AddStep(this);
            Step.AddStep(Expr.Assign(this, value));
        }

        #region 基础数据类型隐式转换
        public static implicit operator Var(ParamExpression value) => new Var(value);
        public static implicit operator Var(InstanceExpression value) => new Var(value);
        public static implicit operator Var(MethodExpression value) => new Var(value);
        public static implicit operator Var(ConvertExpression value) => new Var(value);
        public static implicit operator Var(ConstantExpression value) => new Var(value);

        public static implicit operator Var(string value) =>new Var(Expr.Constant(value));

        public static implicit operator Var(long value) => new Var(Expr.Constant(value));

        public static implicit operator Var(ulong value) => new Var(Expr.Constant(value));

        public static implicit operator Var(int value) => new Var(Expr.Constant(value));

        public static implicit operator Var(uint value) => new Var(Expr.Constant(value));

        public static implicit operator Var(short value) => new Var(Expr.Constant(value));

        public static implicit operator Var(ushort value) => new Var(Expr.Constant(value));

        public static implicit operator Var(byte value) => new Var(Expr.Constant(value));

        public static implicit operator Var(sbyte value) => new Var(Expr.Constant(value));

        public static implicit operator Var(nint value) => new Var(Expr.Constant(value));

        public static implicit operator Var(nuint value) => new Var(Expr.Constant(value));

        public static implicit operator Var(float value) => new Var(Expr.Constant(value));

        public static implicit operator Var(double value) => new Var(Expr.Constant(value));

        public static implicit operator Var(decimal value) => new Var(Expr.Constant(value));

        public static implicit operator Var(bool value) => new Var(Expr.Constant(value));

        public static implicit operator Var(char value) => new Var(Expr.Constant(value));
        #endregion

        #region 运算符重载  
        public static Var operator --(Var value)
        {
            value.Step.AddStep(value.PreDecrementAssign());
            return value;
        }
        public static Var operator ++(Var value)
        {
            value.Step.AddStep(value.PreIncrementAssign());
            return value;
        }
        #endregion
    }
    public partial class Var
    {
        /// <summary>
        /// 获取成员（包含字段、属性、方法、索引器）
        /// <para>使用[""]可以获取索引器</para>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <param name="methodParamTypes">由于方法有重载的特性，所以需要指定参数类型</param>
        /// <returns></returns>
        public CommonValueExpression MemberQuery(string memberName, params Type[] methodParamTypes)
        {
            if (memberName.StartsWith('[') && memberName.EndsWith(']'))//[]说明是索引器
            {
                var index = memberName.TrimStart('[').TrimEnd(']');
                if (int.TryParse(index, out var intIndex)) return this.Method("get_Item", intIndex);//Member(new object[] { intIndex });
                else return this.Method("get_Item", index);//Member(new[] { index });
            }
            var expr = MemberQueryExpression.MemberQuery(this,memberName, methodParamTypes);
            return expr;
        }
        /// <summary>
        /// 获取索引器
        /// </summary>
        /// <param name="propertyIndexParams">索引器的indexs</param>
        /// <returns></returns>
        public CommonValueExpression MemberQuery(IEnumerable<object> propertyIndexParams)
        {
            var expr = MemberQueryExpression.MemberQuery(this,propertyIndexParams.ToExpr());
            return expr;
        }
        /// <summary>
        /// 成员赋值（包含字段、属性）
        /// </summary>
        /// <param name="value">值</param>
        public void MemberBind(CommonValueExpression value)
        {
            var memberName = value.Name;
            if (memberName.StartsWith('[') && memberName.EndsWith(']'))
            {
                var index = memberName.TrimStart('[').TrimEnd(']');
                if (int.TryParse(index, out var intIndex)) BlockMethod("set_Item", intIndex,value);//MemberBind(value, new object[] { intIndex });
                else BlockMethod("set_Item", index, value);//MemberBind(value, new[] { index });
            }
            else
            {
                Step.AddStep(MemberBindExpression.MemberBind(this,value));
            }
        }
        /// <summary>
        /// 索引器赋值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="propertyIndexParams">索引器的indexs</param>
        public void MemberBind(CommonValueExpression value, IEnumerable<object> propertyIndexParams)
        {
            Step.AddStep(MemberBindExpression.MemberBind(this,value, propertyIndexParams.ToExpr()));
        }
        /// <summary>
        /// 获取属性\设置属性
        /// <para>注意：获取属性有可能会获取到空参数的方法</para>
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public new CommonValueExpression this[string memberName]
        {
            get
            {
                return MemberQuery(memberName);
            }
            set
            {
                value.Name = memberName;
                MemberBind(value);
            }
        }
        /// <summary>
        /// 索引器获取\赋值
        /// </summary>
        /// <param name="propertyIndexParams"></param>
        /// <returns></returns>
        public new CommonValueExpression this[params object[] propertyIndexParams]
        {
            get
            {
                return MemberQuery(propertyIndexParams);
            }
            set
            {
                MemberBind(value, propertyIndexParams);
            }
        }

        /// <summary>
        /// 会加入方法块
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public void BlockMethod(string methodName, params CommonValueExpression[] param)
        {
            Step.AddStep(this.Method(methodName, param));
        }
        
        public void Assgin(CommonValueExpression value)
        {
            Step.AddStep(Expr.Assign(this, value));
        }
        public override TDelegate BuildDelegate<TDelegate>()
        {
            ExprStepsContainer.RemoveExprStep(Step);
            var steps = Step.GetSteps();        
            var block = Expr.BuildBlockExpr(steps, this);
            return block.BuildDelegate<TDelegate>();
        }
        public TDelegate BuildDefaultDelegate<TDelegate>(CommonValueExpression? rtValue=null) where TDelegate : Delegate
        {
            ExprStepsContainer.RemoveExprStep(Step);
            var steps = Step.GetSteps();
            var block = Expr.BuildBlockExpr(steps, rtValue);
            return block.BuildDelegate<TDelegate>();
        }
    }
}
