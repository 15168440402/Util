using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Util.ORM.Util
{
    internal static class DbCommandExtension
    {
        public static async Task<int> ExecuteNonQueryAsync(this IDbCommand dbCommand)
        {
            if (dbCommand is DbCommand cmd) return await cmd.ExecuteNonQueryAsync();
            else return dbCommand.ExecuteNonQuery();
        }
        public static async Task<object?> ExecuteScalarAsync(this IDbCommand dbCommand)
        {
            if (dbCommand is DbCommand cmd) return await cmd.ExecuteScalarAsync();
            else return dbCommand.ExecuteScalar();
        }
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand dbCommand, CommandBehavior behavior)
        {
            if (dbCommand is DbCommand cmd) return await cmd.ExecuteReaderAsync(behavior);
            else return dbCommand.ExecuteReader(behavior);
        }
    }
}
