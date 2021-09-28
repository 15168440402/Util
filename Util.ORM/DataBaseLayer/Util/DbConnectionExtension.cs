using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Util.ORM.Util
{
    internal static class DbConnectionExtension
    {
        /// <summary>
        /// 当IDbConnection连接状态为关闭的时候开启连接（异步）
        /// <para>目的阐述：连接状态开启的时候再开启会报错，此方法可以避免这个问题</para>
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static async Task WhenCloseOpenAsync(this IDbConnection dbConnection)
        {
            if (dbConnection.State == ConnectionState.Open) return;
            if (dbConnection is DbConnection connection) await connection.OpenAsync();
            else dbConnection.Open();
        }
        /// <summary>
        /// 当IDbConnection连接状态为关闭的时候开启连接
        /// <para>目的阐述：连接状态开启的时候再开启会报错，此方法可以避免这个问题</para>
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static void WhenCloseOpen(this IDbConnection dbConnection)
        {
            if (dbConnection.State == ConnectionState.Open) return;
            dbConnection.Open();
        }
        /// <summary>
        /// 构建IDbCommand
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static IDbCommand BuilderCommand(this IDbConnection dbConnection, CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters, int commandTimeout, IDbTransaction? transaction)
        {
            var cmd = dbConnection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = commandTimeout;
            cmd.Transaction = transaction;

            if (parameters is not null)
            {
                foreach (var paramter in parameters)
                {
                    cmd.Parameters.Add(paramter);
                }
            }
            return cmd;
        }              
    }   
}
