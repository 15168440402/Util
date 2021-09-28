using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Reflection.Expressions
{
    internal static class ExprStepsContainer
    {
        readonly static ConcurrentDictionary<string, ExprStep> _map;
        static ExprStepsContainer()
        {
            _map = new ConcurrentDictionary<string, ExprStep>();
        }
        public static ExprStep QueryExprStep()
        {
            var index=Thread.CurrentThread.ManagedThreadId.ToString();
            return _map.GetOrAdd(index, i => new ExprStep(i));
        }
        public static void RemoveExprStep(ExprStep step)
        {
            _map.Remove(step.Index,out var _);
        }
    }
    internal class ExprStep
    {
        public ExprStep(string index)
        {
            Index = index;
            Level = new Level(index);
            CurrentSteps = new List<CommonExpression>();
            StepsMap = new Dictionary<string, List<CommonExpression>>();
            StepsMap.Add(index,CurrentSteps);
        }
        public string Index { get; set; }
        Level Level { get; set; }
        List<CommonExpression> CurrentSteps { get; set; }
        Dictionary<string, List<CommonExpression>> StepsMap { get; init; }
        public void AddStep(CommonExpression expr)
        {
            CurrentSteps.Add(expr);
        }
        public List<CommonExpression> GetSteps()
        {
            var output = CurrentSteps;
            if (Level.PrevLevel is not null)
            {
                Level = Level.PrevLevel;
                CurrentSteps = StepsMap[Level.Index];
            }
            return output;
        }
        public void SwitchLevel(string index)
        {
            CurrentSteps = new List<CommonExpression>();
            StepsMap[index]=CurrentSteps;
            Level = new Level(index, Level);
        }
    }
    class Level
    {
        public Level(string index)
        {
            Index = index;
        }
        public Level(string index, Level prevLevel)
        {
            Index = index;
            PrevLevel = prevLevel;
        }
        public Level? PrevLevel { get; set; }
        public string Index { get; }
    }
}
