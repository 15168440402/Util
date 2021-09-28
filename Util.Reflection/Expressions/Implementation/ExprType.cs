using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Reflection.Expressions
{
    public enum ExprType
    {
        /// <summary>
        /// 运算符+
        /// </summary>
        Add = 0,
        //
        // 摘要:
        //     An addition operation, such as (a + b), with overflow checking, for numeric operands.
        AddChecked = 1,
        //
        // 摘要:
        //     A bitwise or logical AND operation, such as (a & b) in C# and (a And b) in Visual
        //     Basic.
        And = 2,
        //
        // 摘要:
        //     A conditional AND operation that evaluates the second operand only if the first
        //     operand evaluates to true. It corresponds to (a && b) in C# and (a AndAlso b)
        //     in Visual Basic.
        AndAlso = 3,
        //
        // 摘要:
        //     An operation that obtains the length of a one-dimensional array, such as array.Length.
        ArrayLength = 4,
        //
        // 摘要:
        //     An indexing operation in a one-dimensional array, such as array[index] in C#
        //     or array(index) in Visual Basic.
        ArrayIndex = 5,
        /// <summary>
        /// 调用方法
        /// <para>阐述：用于模拟调用静态类、实例的方法</para> 
        /// </summary>
        Call = 6,
        //
        // 摘要:
        //     A node that represents a null coalescing operation, such as (a ?? b) in C# or
        //     If(a, b) in Visual Basic.
        Coalesce = 7,
        //
        // 摘要:
        //     A conditional operation, such as a > b ? a : b in C# or If(a > b, a, b) in Visual
        //     Basic.
        Conditional = 8,
        //
        // 摘要:
        //     A constant value.
        Constant = 9,
        //
        // 摘要:
        //     A cast or conversion operation, such as (SampleType)obj in C#or CType(obj, SampleType)
        //     in Visual Basic. For a numeric conversion, if the converted value is too large
        //     for the destination type, no exception is thrown.
        Convert = 10,
        //
        // 摘要:
        //     A cast or conversion operation, such as (SampleType)obj in C#or CType(obj, SampleType)
        //     in Visual Basic. For a numeric conversion, if the converted value does not fit
        //     the destination type, an exception is thrown.
        ConvertChecked = 11,
        //
        // 摘要:
        //     A division operation, such as (a / b), for numeric operands.
        Divide = 12,
        //
        // 摘要:
        //     A node that represents an equality comparison, such as (a == b) in C# or (a =
        //     b) in Visual Basic.
        Equal = 13,
        //
        // 摘要:
        //     A bitwise or logical XOR operation, such as (a ^ b) in C# or (a Xor b) in Visual
        //     Basic.
        ExclusiveOr = 14,
        //
        // 摘要:
        //     A "greater than" comparison, such as (a > b).
        GreaterThan = 15,
        //
        // 摘要:
        //     A "greater than or equal to" comparison, such as (a >= b).
        GreaterThanOrEqual = 16,
        //
        // 摘要:
        //     An operation that invokes a delegate or lambda expression, such as sampleDelegate.Invoke().
        Invoke = 17,
        //
        // 摘要:
        //     A lambda expression, such as a => a + a in C# or Function(a) a + a in Visual
        //     Basic.
        Lambda = 18,
        //
        // 摘要:
        //     A bitwise left-shift operation, such as (a << b).
        LeftShift = 19,
        //
        // 摘要:
        //     A "less than" comparison, such as (a < b).
        LessThan = 20,
        //
        // 摘要:
        //     A "less than or equal to" comparison, such as (a <= b).
        LessThanOrEqual = 21,
        //
        // 摘要:
        //     An operation that creates a new System.Collections.IEnumerable object and initializes
        //     it from a list of elements, such as new List<SampleType>(){ a, b, c } in C# or
        //     Dim sampleList = { a, b, c } in Visual Basic.
        ListInit = 22,
        //
        // 摘要:
        //     An operation that reads from a field or property, such as obj.SampleProperty.
        MemberAccess = 23,
        //
        // 摘要:
        //     An operation that creates a new object and initializes one or more of its members,
        //     such as new Point { X = 1, Y = 2 } in C# or New Point With {.X = 1, .Y = 2} in
        //     Visual Basic.
        MemberInit = 24,
        //
        // 摘要:
        //     An arithmetic remainder operation, such as (a % b) in C# or (a Mod b) in Visual
        //     Basic.
        Modulo = 25,
        //
        // 摘要:
        //     A multiplication operation, such as (a * b), without overflow checking, for numeric
        //     operands.
        Multiply = 26,
        //
        // 摘要:
        //     An multiplication operation, such as (a * b), that has overflow checking, for
        //     numeric operands.
        MultiplyChecked = 27,
        //
        // 摘要:
        //     An arithmetic negation operation, such as (-a). The object a should not be modified
        //     in place.
        Negate = 28,
        //
        // 摘要:
        //     A unary plus operation, such as (+a). The result of a predefined unary plus operation
        //     is the value of the operand, but user-defined implementations might have unusual
        //     results.
        UnaryPlus = 29,
        //
        // 摘要:
        //     An arithmetic negation operation, such as (-a), that has overflow checking. The
        //     object a should not be modified in place.
        NegateChecked = 30,
        /// <summary>
        /// 创建实例
        /// <para>阐述：用于模拟实例化类</para>
        /// </summary>
        New = 31,
        //
        // 摘要:
        //     An operation that creates a new one-dimensional array and initializes it from
        //     a list of elements, such as new SampleType[]{a, b, c} in C# or New SampleType(){a,
        //     b, c} in Visual Basic.
        NewArrayInit = 32,
        //
        // 摘要:
        //     An operation that creates a new array, in which the bounds for each dimension
        //     are specified, such as new SampleType[dim1, dim2] in C# or New SampleType(dim1,
        //     dim2) in Visual Basic.
        NewArrayBounds = 33,
        //
        // 摘要:
        //     A bitwise complement or logical negation operation. In C#, it is equivalent to
        //     (~a) for integral types and to (!a) for Boolean values. In Visual Basic, it is
        //     equivalent to (Not a). The object a should not be modified in place.
        Not = 34,
        //
        // 摘要:
        //     An inequality comparison, such as (a != b) in C# or (a <> b) in Visual Basic.
        NotEqual = 35,
        //
        // 摘要:
        //     A bitwise or logical OR operation, such as (a | b) in C# or (a Or b) in Visual
        //     Basic.
        Or = 36,
        //
        // 摘要:
        //     A short-circuiting conditional OR operation, such as (a || b) in C# or (a OrElse
        //     b) in Visual Basic.
        OrElse = 37,
        //
        // 摘要:
        //     A reference to a parameter or variable that is defined in the context of the
        //     expression. For more information, see System.Linq.Expressions.ParameterExpression.
        Parameter = 38,
        //
        // 摘要:
        //     A mathematical operation that raises a number to a power, such as (a ^ b) in
        //     Visual Basic.
        Power = 39,
        //
        // 摘要:
        //     An expression that has a constant value of type System.Linq.Expressions.Expression.
        //     A System.Linq.Expressions.ExpressionType.Quote node can contain references to
        //     parameters that are defined in the context of the expression it represents.
        Quote = 40,
        //
        // 摘要:
        //     A bitwise right-shift operation, such as (a >> b).
        RightShift = 41,
        //
        // 摘要:
        //     A subtraction operation, such as (a - b), without overflow checking, for numeric
        //     operands.
        Subtract = 42,
        //
        // 摘要:
        //     An arithmetic subtraction operation, such as (a - b), that has overflow checking,
        //     for numeric operands.
        SubtractChecked = 43,
        //
        // 摘要:
        //     An explicit reference or boxing conversion in which null is supplied if the conversion
        //     fails, such as (obj as SampleType) in C# or TryCast(obj, SampleType) in Visual
        //     Basic.
        TypeAs = 44,
        //
        // 摘要:
        //     A type test, such as obj is SampleType in C# or TypeOf obj is SampleType in Visual
        //     Basic.
        TypeIs = 45,
        //
        // 摘要:
        //     An assignment operation, such as (a = b).
        Assign = 46,
        //
        // 摘要:
        //     A block of expressions.
        Block = 47,
        //
        // 摘要:
        //     Debugging information.
        DebugInfo = 48,
        //
        // 摘要:
        //     A unary decrement operation, such as (a - 1) in C# and Visual Basic. The object
        //     a should not be modified in place.
        Decrement = 49,
        //
        // 摘要:
        //     A dynamic operation.
        Dynamic = 50,
        //
        // 摘要:
        //     A default value.
        Default = 51,
        //
        // 摘要:
        //     An extension expression.
        Extension = 52,
        //
        // 摘要:
        //     A "go to" expression, such as goto Label in C# or GoTo Label in Visual Basic.
        Goto = 53,
        //
        // 摘要:
        //     A unary increment operation, such as (a + 1) in C# and Visual Basic. The object
        //     a should not be modified in place.
        Increment = 54,
        //
        // 摘要:
        //     An index operation or an operation that accesses a property that takes arguments.
        Index = 55,
        //
        // 摘要:
        //     A label.
        Label = 56,
        //
        // 摘要:
        //     A list of run-time variables. For more information, see System.Linq.Expressions.RuntimeVariablesExpression.
        RuntimeVariables = 57,
        //
        // 摘要:
        //     A loop, such as for or while.
        Loop = 58,
        //
        // 摘要:
        //     A switch operation, such as switch in C# or Select Case in Visual Basic.
        Switch = 59,
        //
        // 摘要:
        //     An operation that throws an exception, such as throw new Exception().
        Throw = 60,
        //
        // 摘要:
        //     A try-catch expression.
        Try = 61,
        //
        // 摘要:
        //     An unbox value type operation, such as unbox and unbox.any instructions in MSIL.
        Unbox = 62,
        //
        // 摘要:
        //     An addition compound assignment operation, such as (a += b), without overflow
        //     checking, for numeric operands.
        AddAssign = 63,
        //
        // 摘要:
        //     A bitwise or logical AND compound assignment operation, such as (a &= b) in C#.
        AndAssign = 64,
        //
        // 摘要:
        //     An division compound assignment operation, such as (a /= b), for numeric operands.
        DivideAssign = 65,
        //
        // 摘要:
        //     A bitwise or logical XOR compound assignment operation, such as (a ^= b) in C#.
        ExclusiveOrAssign = 66,
        //
        // 摘要:
        //     A bitwise left-shift compound assignment, such as (a <<= b).
        LeftShiftAssign = 67,
        //
        // 摘要:
        //     An arithmetic remainder compound assignment operation, such as (a %= b) in C#.
        ModuloAssign = 68,
        //
        // 摘要:
        //     A multiplication compound assignment operation, such as (a *= b), without overflow
        //     checking, for numeric operands.
        MultiplyAssign = 69,
        //
        // 摘要:
        //     A bitwise or logical OR compound assignment, such as (a |= b) in C#.
        OrAssign = 70,
        //
        // 摘要:
        //     A compound assignment operation that raises a number to a power, such as (a ^=
        //     b) in Visual Basic.
        PowerAssign = 71,
        //
        // 摘要:
        //     A bitwise right-shift compound assignment operation, such as (a >>= b).
        RightShiftAssign = 72,
        //
        // 摘要:
        //     A subtraction compound assignment operation, such as (a -= b), without overflow
        //     checking, for numeric operands.
        SubtractAssign = 73,
        //
        // 摘要:
        //     An addition compound assignment operation, such as (a += b), with overflow checking,
        //     for numeric operands.
        AddAssignChecked = 74,
        //
        // 摘要:
        //     A multiplication compound assignment operation, such as (a *= b), that has overflow
        //     checking, for numeric operands.
        MultiplyAssignChecked = 75,
        //
        // 摘要:
        //     A subtraction compound assignment operation, such as (a -= b), that has overflow
        //     checking, for numeric operands.
        SubtractAssignChecked = 76,
        //
        // 摘要:
        //     A unary prefix increment, such as (++a). The object a should be modified in place.
        PreIncrementAssign = 77,
        //
        // 摘要:
        //     A unary prefix decrement, such as (--a). The object a should be modified in place.
        PreDecrementAssign = 78,
        //
        // 摘要:
        //     A unary postfix increment, such as (a++). The object a should be modified in
        //     place.
        PostIncrementAssign = 79,
        //
        // 摘要:
        //     A unary postfix decrement, such as (a--). The object a should be modified in
        //     place.
        PostDecrementAssign = 80,
        //
        // 摘要:
        //     An exact type test.
        TypeEqual = 81,
        //
        // 摘要:
        //     A ones complement operation, such as (~a) in C#.
        OnesComplement = 82,
        //
        // 摘要:
        //     A true condition value.
        IsTrue = 83,
        //
        // 摘要:
        //     A false condition value.
        IsFalse = 84,
        /// <summary>
        /// 自定义Operation节点
        /// <para>阐述：用于模拟运算符操作</para>
        /// </summary>
        Operation = 85,
        ///// <summary>
        ///// 自定义Empty节点
        ///// <para>阐述：用于代替null，杜绝null带来的代码隐患</para>
        ///// </summary>
        //Empty = -1,
        Variable = 86,
        /// <summary>
        /// 成员查询
        /// </summary>
        MemberQuery,
        /// <summary>
        /// 成员赋值
        /// </summary>
        MemberBind,
        Method,
        Continue,
        Return,
        Break
    }
}
