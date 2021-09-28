using System.Collections.Generic;
using System.Linq;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Reflection.Expressions.IntelligentGeneration.Extensions
{
    public static class ObjectExtension
    {
        public static CommonValueExpression ToExpr(this object input, string name = "")
        {
            if (input is CommonValueExpression value) return value;
            else return Expr.Constant(input);
        }
        public static IEnumerable<CommonValueExpression> ToExpr(this IEnumerable<object> input)
        {
            return input.Select(o => o.ToExpr());
        }
    }
}
