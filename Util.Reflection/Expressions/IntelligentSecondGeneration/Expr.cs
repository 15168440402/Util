using System;
using System.Collections.Generic;
using System.Linq;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Reflection.Expressions
{
    public partial class Expr
    {
        public static Var Var(CommonValueExpression value) => new Var(value);
        public static ParamExpression BlockParam(Type type)//后期会删除此功能，只需要用Expr.Param即可
        {
            var step = ExprStepsContainer.QueryExprStep();
            var paramter = Expr.Param(type);
            step.AddStep(paramter);
            return paramter;
        }
        public static ParamExpression BlockParam<T>()//后期会删除此功能，只需要用Expr.Param即可
        {
            return BlockParam(typeof(T));
        }
        internal static CommonExpression BuildBlockExpr(IEnumerable<CommonExpression> exprs, CommonValueExpression? returnExp = null)
        {
            if (exprs.Count() == 1) return exprs.First();
            var variables = exprs.Where(expr => expr is CommonVariableExpression).Select(expr => (CommonVariableExpression)expr);
            var expression = exprs.Where(expr => expr is not CommonVariableExpression);
            var block = Expr.Block(variables, expression, returnExp);
            return block;
        }
        public static void IfThen(CommonValueExpression conditional, Action ifTrueAction)
        {
            var step = ExprStepsContainer.QueryExprStep();
            string index = Guid.NewGuid().ToString();
            step.SwitchLevel(index);
            ifTrueAction();
            var steps = step.GetSteps();
            var ifThenExpr = Expr.IfThen(conditional, BuildBlockExpr(steps));
            step.AddStep(ifThenExpr);
        }

        public static void IfThenElse(CommonValueExpression conditional, Action ifTrueAction, Action ifFalseAction)
        {
            var step = ExprStepsContainer.QueryExprStep();
            string index = Guid.NewGuid().ToString();
            step.SwitchLevel(index);
            ifTrueAction();
            var trueSteps = step.GetSteps();
            step.SwitchLevel(index);
            ifFalseAction();
            var falseSteps = step.GetSteps();
            var ifThenElseExpr = Expr.IfThenElse(conditional, BuildBlockExpr(trueSteps), BuildBlockExpr(falseSteps));
            step.AddStep(ifThenElseExpr);
        }
        public static void While(CommonValueExpression conditional, Action<Action, Action> body)
        {
            var step = ExprStepsContainer.QueryExprStep();
            string index = Guid.NewGuid().ToString();
            step.SwitchLevel(index);
            var continueExpr = Expr.Continue(Lable.VoidTarget);
            var returnExpr = Expr.Return(Lable.VoidTarget);
            Action continueAction = () => step.AddStep(continueExpr);
            Action returnAction = () => step.AddStep(returnExpr);
            body(continueAction, returnAction);
            var bodySteps = step.GetSteps();
            var ifThenExpr = Expr.IfThenElse(conditional, BuildBlockExpr(bodySteps), returnExpr);
            var loopExpr = Expr.Loop(ifThenExpr, returnExpr.Target, continueExpr.Target);
            step.AddStep(loopExpr);
        }
        public static void DoWhile(Action body, CommonValueExpression conditional)
        {
            body();
            While(conditional, (c, r) =>
            {
                body();
            });
        }
        public static void Foreach(CommonValueExpression enumerable, Action<CommonValueExpression, Action, Action> body)
        {
            var enumeratorGen = enumerable.Method("GetEnumerator");
            if(enumeratorGen.Type.GetMethod("MoveNext") is not null)
            {
                Var enumerator = enumeratorGen;
                While(enumerator.Method("MoveNext"), (c, r) =>
                {
                    body(enumerator["Current"], c, r);
                });
            }
            else
            {
                var type = enumeratorGen.Type.GenericTypeArguments.First();
                Var enumerator = enumeratorGen.Convert<System.Collections.IEnumerator>();
                While(enumerator.Method("MoveNext"), (c, r) =>
                {
                    Var convert = enumerator["Current"].Convert(type);
                    body(convert, c, r);
                });
            }            
        }
        public static void For(int endIndex,Action<CommonValueExpression,Action, Action> body)
        {
            Var i = 0;
            var conditional = i < endIndex;
            While(conditional, (c, r) =>
            {
                i++;
                body(i,c,r);
            });
        }
    }
}
