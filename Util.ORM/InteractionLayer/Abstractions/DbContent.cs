using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Util.ORM.InteractionLayer.SqlCmd;

namespace Util.ORM.Abstractions
{
    public  class DbContent//abstract
    {
        //DBHelper DBHelper { get; init; }
        //public DbContent(IDbConnection dbConnection)
        //{
        //    DBHelper = new DBHelper(dbConnection);
        //}
        public From<Table> From<Table>() => new From<Table>();
        public void From<T1, T2>(Expression<Func<T1, T2, bool>> func)
        {
            var c = func;
        }
        public void From<T1, T2>(Expression<Func<T1,T2, Join, Join>> func)
        {
            var c = func;
        }
        public SelectFrom<T> From<T>(Select<T> select) => new SelectFrom<T>(select);
    }
}
