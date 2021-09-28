using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Util.ORM.Abstractions;

namespace Util.ORM
{
    public static class DbOperateExtension
    {
        public static IFrom<Table> Where<Table>(this IFrom<Table> from,Expression<Func<Table, bool>> func)
        {
            return from;
        }
        public static ISelect<Table> Select<Table, TResult>(this ISelect<Table> selectOperate, Expression<Func<Table, TResult>> func)
        {
            return selectOperate;
        }

    }
}
