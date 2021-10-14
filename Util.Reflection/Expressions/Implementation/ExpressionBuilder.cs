using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Util.Reflection.Expressions.Abstractions;
using Util.Reflection.Types;

namespace Util.Reflection.Expressions
{
    #region 参数表达式
    /// <summary>
    /// 标识为参数
    /// <para>在委托表达式中充当参数角色</para>
    /// <para>注解：参数指委托<see cref="Action{T}"/>中的T参数/<see cref="Func{T, TResult}"/>中的T参数</para>
    /// <para>备注：对应表达式中的<see cref="ParameterExpression"/></para>
    /// </summary>
    public partial class ParamExpression : CommonValueExpression
    {
        public ParamExpression(Type type, string name = "") : base(type)
        {
            Name = name;
        }
        public override ExprType NodeType { get; protected set; } = ExprType.Parameter;

        private protected override Expression ConvertToExpression()
        {
            return Expression.Parameter(Type, Name);
        }
    }
    #endregion

    #region 值表达式
    /// <summary>
    /// 标识为值
    /// <para>注意：1.建议使用值类型，使用引用类型会使所有委托表达式共享这个引用类型的指针。</para>
    /// <para>2.如果要创建/实例化委托表达式中的引用类型，请使用<see cref="InstanceExpression"/>。</para>
    /// <para>备注：对应表达式里的<see cref="System.Linq.Expressions.ConstantExpression"/></para>
    /// </summary>
    public partial class ConstantExpression : CommonValueExpression
    {
        public ConstantExpression(object? value, Type type, string name = "") : base(type) => (Value, Type, Name) = (value, type, name);
        public object? Value { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.Constant;

        private protected override Expression ConvertToExpression()
        {
            return Expression.Constant(Value, Type);
        }

        public static ConstantExpression Constant(object? value, Type type, string name = "") => new ConstantExpression(value, type, name);
        public static ConstantExpression Constant(object value, string name = "") => Constant(value, value.GetType(), name);
    }
    #endregion

    #region 变量定义表达式
    /// <summary>
    /// 标识为变量
    /// <para>在委托表达式中充当变量角色</para>
    /// <para>注解：变量指我们c#语言平时使用的变量,比如以下代码：</para>
    /// <code>string excample="I is Variable";</code>
    /// <para>excample就是变量</para>
    /// <para>备注：对应表达式中的<see cref="ParameterExpression"/></para>
    /// </summary>
    public partial class VariableExpression : CommonVariableExpression
    {
        public VariableExpression(Type type, string name = "") : base(type)
        {
            Name = name;
        }

        public override ExprType NodeType { get; protected set; } = ExprType.Variable;

        private protected override ParameterExpression ConvertToExpression()
        {
            return Expression.Variable(Type);
        }
    }
    #endregion

    #region block方法块、方法体  
    /// <summary>
    /// 标识为方法体
    /// <para>注解：方法体就是多行代码的表达式，比如以下代码：</para>
    /// <code>string ls=new List();int count=ls.Count;</code>
    /// <para>备注：对应表达式中的<see cref="BlockExpression"/></para>
    /// </summary>
    public partial class BodyExpression : CommonValueExpression
    {
        public BodyExpression(IEnumerable<CommonVariableExpression>? vars, IEnumerable<CommonExpression> units, CommonValueExpression? returnExp) : base(returnExp?.Type ?? typeof(void)) => (Vars, Units, ReturnExp) = (vars, units, returnExp);
        public IEnumerable<CommonVariableExpression>? Vars { get; set; }
        public IEnumerable<CommonExpression> Units { get; set; }
        public CommonValueExpression? ReturnExp { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.Block;
        private protected override Expression ConvertToExpression()
        {
            var unitExprs = Units.Select(u => (Expression)u).ToList();
            if (ReturnExp is not null) unitExprs.Add(ReturnExp);
            if (Vars is not null)
            {
                var paramters = Vars.Select(v => v.GetExpression());
                return Expression.Block(paramters, unitExprs);
            }
            else return Expression.Block(unitExprs);
        }
    }
    public partial class BodyExpression
    {
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
    }
    #endregion

    #region 实例化类表达式 
    public partial class InstanceExpression : CommonValueExpression
    {
        public InstanceExpression(Type type, IEnumerable<CommonValueExpression> constructorParams) : base(type) => ConstructorParams = constructorParams;
        public IEnumerable<CommonValueExpression> ConstructorParams { get; set; }
        public List<CommonValueExpression> Members { get; set; } = new List<CommonValueExpression>();
        public InstanceExpression InitMember(IEnumerable<CommonValueExpression> members)
        {
            Members.AddRange(members);
            return this;
        }
        public InstanceExpression InitMember(params CommonValueExpression[] members)
        {
            return InitMember(members as IEnumerable<CommonValueExpression>);
        }
        public override ExprType NodeType { get; protected set; } = ExprType.New;
        private protected override Expression ConvertToExpression()
        {
            var type = Type;
            var members = Members;
            var newExp = default(NewExpression);
            if (ConstructorParams.Count() == 0) newExp = Expression.New(Type);
            else
            {
                var types = ConstructorParams.Select(cp => cp.Type).ToArray();
                var constructorInfo = type.GetConstructor(types) ?? throw new UtilException($"类型{type.FullName}不存在构造函数{type.Name}({string.Join(',', types.Select(t => t.Name))})", $"仔细核对下{type.Name}的构造函数");
                newExp = Expression.New(constructorInfo, ConstructorParams.Select(cp => (Expression)cp));
            }

            if (members.Count <= 0) return newExp;
            else
            {
                var memberBindings = new List<MemberBinding>();
                foreach (var member in members)
                {
                    var memberInfo = type.GetMember(member.Name);
                    if (memberInfo.Count() == 0) throw new UtilException($"类型{type.FullName}不存在成员{member.Name}", $"1.请检查{member.Name}是否拼错 2.请检查是否指定错了类型{type.FullName}");
                    memberBindings.Add(Expression.Bind(memberInfo[0], member));
                }
                MemberInitExpression memberInitExp = Expression.MemberInit(newExp, memberBindings);
                return memberInitExp;
            }
        }
    }
    #endregion

    public class MemberParamter
    {
        public MemberParamter(CommonValueExpression instance, string memberName, IEnumerable<Type>? MethodParamTypes = null, IEnumerable<Type>? genericParamTypes = null)
        {
            MemberName = memberName == "" ? "Item" : memberName;
            ClassType = instance.Type;
            MemberDescriptor = TypeDescriptor.QueryMember(ClassType, MemberName, MethodParamTypes, genericParamTypes) ?? throw new UtilException($"类型{ClassType.FullName}不存在属性或字段或方法{MemberName}");
            Instance = instance;
            ConvertInstance = instance;
        }
        public MemberParamter(IStatic @static, string memberName, IEnumerable<Type>? methodParamTypes = null, IEnumerable<Type>? genericParamTypes = null) : this(ConstantExpression.Constant(null, @static.Type), memberName, methodParamTypes, genericParamTypes)
        {
            ConvertInstance = null;
        }
        public string MemberName { get; }
        public Type ClassType { get; init; }
        public MemberDescriptor MemberDescriptor { get; init; }
        public CommonValueExpression Instance { get; init; }
        public Expression? ConvertInstance { get; init; }
        public IEnumerable<CommonValueExpression>? PropertyIndexParams { get; set; }
    }

    #region 类属性字段获取表达式   
    public partial class MemberQueryExpression : CommonValueExpression
    {
        public MemberQueryExpression(MemberParamter paramter) : base(paramter.MemberDescriptor.MemberType) => MemberParamter = paramter;
        public MemberParamter MemberParamter { get; set; }
        public IEnumerable<CommonValueExpression>? PropertyIndexParams { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.MemberQuery;
        private protected override Expression ConvertToExpression()
        {
            var memberInfo = MemberParamter.MemberDescriptor.Member;

            if (memberInfo is PropertyInfo propertyInfo)
            {
                if (MemberParamter.PropertyIndexParams is null) return Expression.Property(MemberParamter.ConvertInstance, propertyInfo);//MemberParamter.Instance
                else return Expression.Property(MemberParamter.Instance, propertyInfo, MemberParamter.PropertyIndexParams.Select(p => (Expression)p));
            }
            if (memberInfo is FieldInfo fildInfo) return Expression.Field(MemberParamter.Instance, fildInfo);
            if (memberInfo is MethodInfo method)
            {
                var expr = MethodExpression.Method(ConstantExpression.Constant(method), nameof(method.CreateDelegate), ConstantExpression.Constant(Type), MemberParamter.Instance);
                return new ConvertExpression(expr, Type);
            }
            throw new UtilException($"获取成员{MemberParamter.MemberName}失败", $"检测类型{MemberParamter.ClassType}是否存在成员{MemberParamter.MemberName}");
        }
    }
    public partial class MemberQueryExpression
    {
        public static MemberQueryExpression MemberQuery(CommonValueExpression instance, IEnumerable<CommonValueExpression> propertyIndexParams)
        {
            return new MemberQueryExpression(new MemberParamter(instance, "")
            {
                PropertyIndexParams = propertyIndexParams,
            });
        }
        public static MemberQueryExpression MemberQuery(CommonValueExpression instance, string memberName, IEnumerable<Type>? methodParamTypes = null)
        {
            return new MemberQueryExpression(new MemberParamter(instance, memberName, methodParamTypes)
            {

            });
        }
        public static MemberQueryExpression MemberQuery(Type classType, string memberName, IEnumerable<Type>? methodParamTypes = null)
        {
            return new MemberQueryExpression(new MemberParamter(new Static(classType), memberName, methodParamTypes)
            {

            });
        }
    }
    #endregion

    #region 类属性赋值表达式
    public partial class MemberBindExpression : CommonExpression
    {
        public MemberBindExpression(MemberParamter paramter, CommonValueExpression memberBind) => (MemberParamter, Member) = (paramter, memberBind);
        public MemberParamter MemberParamter { get; set; }
        public CommonValueExpression Member { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.MemberBind;
        private protected override Expression ConvertToExpression()
        {
            var expr = new MemberQueryExpression(MemberParamter);
            return Expression.Assign(expr, Member);
        }
    }
    public partial class MemberBindExpression
    {
        public static MemberBindExpression MemberBind(CommonValueExpression instance, CommonValueExpression member, IEnumerable<CommonValueExpression>? propertyIndexParams = null)
        {
            return new MemberBindExpression(new MemberParamter(instance, member.Name)
            {
                PropertyIndexParams = propertyIndexParams,
            }, member);
        }
        public static MemberBindExpression MemberBind(Type classType, CommonValueExpression member)
        {
            return new MemberBindExpression(new MemberParamter(new Static(classType), member.Name)
            {
            }, member);
        }
    }
    #endregion

    #region 类方法调用表达式
    public partial class MethodExpression : CommonValueExpression
    {
        public MethodExpression(MemberParamter paramter, IEnumerable<CommonValueExpression>? @params) : base(paramter.MemberDescriptor.MemberType) => (MemberParamter, Params) = (paramter, @params);
        public MemberParamter MemberParamter { get; set; }
        public IEnumerable<CommonValueExpression>? Params { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.Method;
        private protected override Expression ConvertToExpression()
        {
            var member = MemberParamter.MemberDescriptor.Member;
            if (member is MethodInfo method)
            {
                var paramExps = Params?.Select(p => (Expression)p);
                if (MemberParamter.MemberDescriptor.ExtraParameters is not null)
                {
                    foreach (var (extra, type) in MemberParamter.MemberDescriptor.ExtraParameters)
                    {
                        paramExps = paramExps?.Append(ConstantExpression.Constant(extra, type));
                    }
                }
                return Expression.Call(MemberParamter.ConvertInstance, method, paramExps);
            }
            throw new UtilException();
            //throw new UtilException($"类型{Type.FullName}不存在{MemberParamter.MemberName}({ string.Join(',', Destructor.MethodParamTypes?.Select(p => p.Name)?.ToList()??new List<string>())})方法");
        }
    }
    public partial class MethodExpression
    {
        public static MethodExpression Method(CommonValueExpression instance, string methodName, IEnumerable<CommonValueExpression>? param)
        {
            return new MethodExpression(new MemberParamter(instance, methodName, param?.Select(p => p.Type))
            {
            }, param);
        }
        public static MethodExpression Method(CommonValueExpression instance, string methodName, params CommonValueExpression[] param)
        {
            return Method(instance, methodName, param as IEnumerable<CommonValueExpression>);
        }
        public static MethodExpression Method(Type classType, string methodName, IEnumerable<CommonValueExpression>? param)
        {
            return new MethodExpression(new MemberParamter(new Static(classType), methodName, param?.Select(p => p.Type))
            {
            }, param);
        }
        public static MethodExpression Method(Type instanceType, string methodName, params CommonValueExpression[] param)
        {
            return Method(instanceType, methodName, param as IEnumerable<CommonValueExpression>);
        }
    }
    #endregion

    #region 数据类型转换表达式
    public partial class ConvertExpression : CommonValueExpression
    {
        public CommonValueExpression ConverValue { get; set; }
        public ConvertExpression(CommonValueExpression converValue, Type converType) : base(converType) => ConverValue = converValue;
        public override ExprType NodeType { get; protected set; } = ExprType.Convert;
        private protected override Expression ConvertToExpression()
        {
            return Expression.Convert(ConverValue, Type);
        }
    }
    #endregion

    #region 运算表达式
    public partial class OperationExpression : CommonValueExpression
    {
        public OperationExpression(ExprType nodeType, CommonValueExpression leftExp, CommonValueExpression? rightExp) : base(GetType(nodeType, leftExp.Type, rightExp?.Type)) => (NodeType, LeftExp, RightExp) = (nodeType, leftExp, rightExp);
        public CommonValueExpression LeftExp { get; set; }
        public CommonValueExpression? _rightExp;
        [NotNull]
        public CommonValueExpression? RightExp { get => _rightExp ?? throw new UtilException("二元运算意外出现少了一元参与（二元运算需要两个数据参与）", "你无法解决的错误"); set => _rightExp = value; }
        public override ExprType NodeType { get; protected set; } = ExprType.Operation;
        private protected override Expression ConvertToExpression()
        {
            return NodeType switch
            {
                ExprType.Assign => Expression.Assign(LeftExp, RightExp),
                ExprType.PreDecrementAssign => Expression.PreDecrementAssign(LeftExp),
                ExprType.PostDecrementAssign => Expression.PostDecrementAssign(LeftExp),
                ExprType.PreIncrementAssign => Expression.PreIncrementAssign(LeftExp),
                ExprType.PostIncrementAssign => Expression.PostIncrementAssign(LeftExp),
                ExprType.Add => Add(),
                ExprType.Subtract => Expression.Subtract(LeftExp, RightExp),
                ExprType.Multiply => Expression.Multiply(LeftExp, RightExp),
                ExprType.Divide => Expression.Divide(LeftExp, RightExp),
                ExprType.Equal => Expression.Equal(LeftExp, RightExp),
                ExprType.NotEqual => Expression.NotEqual(LeftExp, RightExp),
                ExprType.LessThan => Expression.LessThan(LeftExp, RightExp),
                ExprType.GreaterThan => Expression.GreaterThan(LeftExp, RightExp),
                ExprType.LessThanOrEqual => Expression.LessThanOrEqual(LeftExp, RightExp),
                ExprType.GreaterThanOrEqual => Expression.GreaterThanOrEqual(LeftExp, RightExp),
                ExprType.AndAlso => Expression.AndAlso(LeftExp, RightExp),
                ExprType.OrElse => Expression.OrElse(LeftExp, RightExp),
                _ => throw new UtilException($"不支持{NodeType}节点转换", "你无法解决的错误")
            };
        }
        Expression Add()
        {
            if (LeftExp.Type == typeof(int))
            {
                if (RightExp.Type == typeof(int))
                {
                    return Expression.Add(LeftExp, RightExp);
                }
                else if (RightExp.Type == (typeof(string)))
                {
                    var concatFunc = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
                    return Expression.Add(LeftExp, MethodExpression.Method(RightExp, "ToString"), concatFunc);
                }
            }
            else if (LeftExp.Type == typeof(string))
            {
                var concatFunc = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
                if (RightExp.Type == typeof(string))
                {
                    return Expression.Add(LeftExp, RightExp, concatFunc);
                }
                else if (RightExp.Type == (typeof(int)))
                {
                    return Expression.Add(LeftExp, MethodExpression.Method(RightExp, "ToString"), concatFunc);
                }
            }
            throw new UtilException($"报错地址：{nameof(OperationExpression)}.{nameof(Add)}()");
        }
        static Type GetType(ExprType nodeType, Type leftType, Type? rightType)
        {
            return nodeType switch
            {
                ExprType.Assign => typeof(void),
                ExprType.Add or ExprType.Subtract or ExprType.Multiply or ExprType.Divide when leftType == typeof(int) && rightType == typeof(int) => typeof(int),
                ExprType.Add when leftType == typeof(string) || rightType == typeof(string) => typeof(string),
                ExprType.PreDecrementAssign or ExprType.PostDecrementAssign or ExprType.PreIncrementAssign or ExprType.PostIncrementAssign when leftType == typeof(int) => typeof(int),
                ExprType.Equal or ExprType.NotEqual or ExprType.LessThan or ExprType.GreaterThan or ExprType.LessThanOrEqual or ExprType.GreaterThanOrEqual => typeof(bool),
                ExprType.AndAlso or ExprType.OrElse => typeof(bool),
                _ => throw new UtilException($"({leftType.FullName},{rightType?.FullName ?? ""})不支持运算符{nodeType}"),
            };
        }
    }
    public partial class OperationExpression
    {
        private static OperationExpression Instantiate(ExprType nodeType, CommonValueExpression left, CommonValueExpression? right = null)
        {
            return new OperationExpression(nodeType, left, right);
        }
        public static OperationExpression Assign(CommonVariableExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.Assign, left, right);
        }
        public static OperationExpression PreDecrementAssign(CommonValueExpression left)
        {
            return Instantiate(ExprType.PreDecrementAssign, left);
        }
        public static OperationExpression PostDecrementAssign(CommonValueExpression left)
        {
            return Instantiate(ExprType.PostDecrementAssign, left);
        }
        public static OperationExpression PreIncrementAssign(CommonValueExpression left)
        {
            return Instantiate(ExprType.PreIncrementAssign, left);
        }
        public static OperationExpression PostIncrementAssign(CommonValueExpression left)
        {
            return Instantiate(ExprType.PostIncrementAssign, left);
        }
        public static OperationExpression Add(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.Add, left, right);
        }
        public static OperationExpression Subtract(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.Subtract, left, right);
        }
        public static OperationExpression Multiply(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.Multiply, left, right);
        }
        public static OperationExpression Divide(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.Divide, left, right);
        }
        public static OperationExpression UnaryPlus(CommonValueExpression left)
        {
            return Instantiate(ExprType.UnaryPlus, left);
        }
        public static OperationExpression Negate(CommonValueExpression left)
        {
            return Instantiate(ExprType.Negate, left);
        }
        public static OperationExpression IsTrue(CommonValueExpression left)
        {
            return Instantiate(ExprType.IsTrue, left);
        }
        public static OperationExpression IsFalse(CommonValueExpression left)
        {
            return Instantiate(ExprType.IsFalse, left);
        }
        public static OperationExpression Not(CommonValueExpression left)
        {
            return Instantiate(ExprType.Not, left);
        }
        public static OperationExpression NotEqual(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.NotEqual, left, right);
        }
        public static OperationExpression Equal(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.Equal, left, right);
        }
        public static OperationExpression LessThan(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.LessThan, left, right);
        }
        public static OperationExpression GreaterThan(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.GreaterThan, left, right);
        }
        public static OperationExpression LessThanOrEqual(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.LessThanOrEqual, left, right);
        }
        public static OperationExpression GreaterThanOrEqual(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.GreaterThanOrEqual, left, right);
        }
        public static OperationExpression AndAlso(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.AndAlso, left, right);
        }
        public static OperationExpression OrElse(CommonValueExpression left, CommonValueExpression right)
        {
            return Instantiate(ExprType.OrElse, left, right);
        }
    }

    #endregion

    #region 条件判断表达式
    public partial class ConditionalExpression : CommonValueExpression
    {
        public ConditionalExpression(CommonValueExpression conditional, CommonExpression ifTrue, CommonExpression? ifFalse) : base(typeof(bool))
        {
            if (conditional.Type != typeof(bool)) throw new UtilException($"If表达式只能比较bool类型，不支持{conditional.Type.Name}类型", "用点脑子");
            (Conditional, IfTrue, IfFalse) = (conditional, ifTrue, ifFalse);
        }
        public CommonValueExpression Conditional { get; set; }
        public CommonExpression IfTrue { get; set; }
        public CommonExpression? IfFalse { get; set; }

        public override ExprType NodeType { get; protected set; } = ExprType.Conditional;
        private protected override Expression ConvertToExpression()
        {
            if (IfFalse is null) return Expression.IfThen(Conditional, IfTrue);
            else return Expression.IfThenElse(Conditional, IfTrue, IfFalse);
        }
    }
    #endregion

    #region 循环表达式
    public class LoopExpression : CommonExpression
    {
        public LoopExpression(CommonExpression body, LabelTarget breakTarget, LabelTarget continueTarget) => (Body, BreakTarget, ContinueTarget) = (body, breakTarget, continueTarget);
        public CommonExpression Body { get; set; }
        public LabelTarget BreakTarget { get; set; }
        public LabelTarget ContinueTarget { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.Loop;
        private protected override Expression ConvertToExpression()
        {
            return Expression.Loop(Body, BreakTarget, ContinueTarget);
        }
    }
    #endregion

    #region goto表达式
    public class ReturnExpression : CommonExpression
    {
        public ReturnExpression(LabelTarget target) => Target = target;
        public LabelTarget Target { get; set; }

        public override ExprType NodeType { get; protected set; } = ExprType.Return;
        private protected override Expression ConvertToExpression()
        {
            return Expression.Return(Target);
        }
    }
    public class ContinueExpression : CommonExpression
    {
        public ContinueExpression(LabelTarget target) => Target = target;
        public LabelTarget Target { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.Continue;
        private protected override Expression ConvertToExpression()
        {
            return Expression.Continue(Target);
        }
    }
    public class BreakExpression : CommonExpression
    {
        public BreakExpression(LabelTarget target) => Target = target;
        public LabelTarget Target { get; set; }
        public override ExprType NodeType { get; protected set; } = ExprType.Break;
        private protected override Expression ConvertToExpression()
        {
            return Expression.Break(Target);
        }
    }
    #endregion

    public class Static : IStatic
    {
        public Static(Type type) => Type = type;
        public Type Type { get; set; }
    }
    /// <summary>
    /// 挺烦的一个东西，goto相关功能需要用到
    /// </summary>
    public static class Lable
    {
        public static LabelTarget VoidTarget => Expression.Label(typeof(void));
    }
}