using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Util.ORM.Abstractions
{
    public class From<Table> : IFrom<Table>
    {
        public From<Table> Where(Expression<Func<Table, bool>> func)
        {
            return this;
        }
        public From<Table> WhereIf(Func<bool> condition, Expression<Func<Table, bool>> func)
        {
            return this;
        }
        public Select<TResult> Select<TResult>(Expression<Func<Table, TResult>> func)
        {
            return new Select<TResult>();
        }
        public Select<TResult> Select<TResult>()
        {
            return new Select<TResult>();
        }
        public async Task<List<Table>> ToListAsync()
        {
            return await this.Select<Table>().ToListAsync();
        }
        public async Task<int> UpdateAsync<TUpdate>(TUpdate tableEntity)
        {
            return await Task.FromResult(1);
        }
        public async Task<int> UpdateAsync<TUpdate>(Expression<Func<TUpdate>> func)
        {
            return await Task.FromResult(1);
        }
        public async Task<int> DeleteAsync()
        {
            return await Task.FromResult(1);
        }
    }
}
