using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Common
{
    public static class Delegate
    {
        public delegate bool Compare<TCompare>(TCompare left, TCompare right);
    }
}
