using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.ORM.Abstractions
{
    public interface IFrom<Table>: IInsert, IDelete, IUpdate, ISelect<Table>
    {
    }
}
