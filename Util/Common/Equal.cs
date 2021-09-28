using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Common
{
    public static class Equal
    {
        public static bool List<TCompare>(IEnumerable<TCompare>? left, IEnumerable<TCompare>? right) where TCompare:struct 
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();
            while (leftEnumerator.MoveNext())
            {
                if (rightEnumerator.MoveNext())
                {
                    if (leftEnumerator.Current.Equals(rightEnumerator.Current) == false) return false;
                }
                else return false;
            }
            if (rightEnumerator.MoveNext()) return false;
            return true;
        }
        public static bool List<TCompare>(IEnumerable<Nullable<TCompare>>? left, IEnumerable<Nullable<TCompare>>? right) where TCompare : struct
        {
            return ListCompare(left, right);
        }
        public static bool ListForClass<TCompare>(IEnumerable<TCompare>? left, IEnumerable<TCompare>? right, Delegate.Compare<TCompare>? compare = null) where TCompare : class
        {

            return ListCompare(left, right);
        }
        private static bool ListCompare<TCompare>(IEnumerable<TCompare>? left, IEnumerable<TCompare>? right, Delegate.Compare<TCompare>? compare = null)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            if (compare is null)
            {
                compare = (left, right) => left!.Equals(right);
            }
            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();
            while (leftEnumerator.MoveNext())
            {
                if (rightEnumerator.MoveNext())
                {
                    if (leftEnumerator.Current is null)
                    {
                        if (rightEnumerator.Current is not null) return false;
                    }
                    else
                    {
                        if (rightEnumerator.Current is null) return false;
                        else
                        {
                            if (leftEnumerator.Current.Equals(rightEnumerator.Current) == false) return false;
                        }
                    }
                }
                else return false;
            }
            if (rightEnumerator.MoveNext()) return false;
            return true;
        }
    }
}
