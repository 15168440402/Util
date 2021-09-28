using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Reflection.Expressions.Abstractions
{
    public interface IDelegteBuilder
    {
        TDelegate BuildDelegate<TDelegate>() where TDelegate : Delegate;
    }
}
