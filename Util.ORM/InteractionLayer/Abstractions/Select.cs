using System.Collections.Generic;
using System.Threading.Tasks;

namespace Util.ORM.Abstractions
{
    public class Select<T> 
    {
        public async Task<List<T>> ToListAsync()
        {
            return await Task.FromResult(new List<T>());
        }
    }
}
