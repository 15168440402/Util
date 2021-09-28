using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Common
{
    public static class Same
    {
        public static List<TCompare> List<TCompare>(IEnumerable<TCompare>? left, IEnumerable<TCompare>? right, Delegate.Compare<TCompare>? compare=null)
        {
            var output = new List<TCompare>();
            if (left is null || right is null) return output;
            if(compare is null)
            {
                compare = (left, right) => left!.Equals(right);
            }
            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();
            while (leftEnumerator.MoveNext())
            {
                if (rightEnumerator.MoveNext())
                {
                    if (leftEnumerator.Current is null || rightEnumerator.Current is null) continue;
                    else
                    {
                        if (compare(leftEnumerator.Current, rightEnumerator.Current)) output.Add(leftEnumerator.Current);
                    }
                }
                else break;
            }
            return output;
        }
    }
}
