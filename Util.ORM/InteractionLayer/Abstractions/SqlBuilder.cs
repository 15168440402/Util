using System.Text;
using System.Collections.Generic;

namespace Util.ORM.Abstractions
{
    public abstract class SqlBuilder
    {
        readonly StringBuilder _sqlStrBuilder;
        public SqlBuilder()
        {
            _sqlStrBuilder = new StringBuilder();
        }
        public virtual void Select(string tableName,IEnumerable<string> columns)
        {
            _sqlStrBuilder.Append($" SELECT {string.Join(',',columns)} FROM {tableName} ");
        }
        public virtual void Where()
        {

        }
    }
}
