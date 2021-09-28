using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Util.ORM.Abstractions
{
    public class SelectFrom<TSelect>
    {
        public SelectFrom(Select<TSelect> select)
        {

        }
        public SelectFrom<TSelect> Where(Expression<Func<TSelect, bool>> func)
        {
            return this;
        }
        public SelectFrom<TSelect> WhereIf(Func<bool> condition, Expression<Func<TSelect, bool>> func)
        {
            return this;
        }
        public Select<TResult> Select<TResult>(Expression<Func<TSelect, TResult>> func)
        {
            return new Select<TResult>();
        }
        public Select<TResult> Select<TResult>()
        {
            return new Select<TResult>();
        }
    }
}
