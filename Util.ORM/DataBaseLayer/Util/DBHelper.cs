using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Util.ORM.Util;

namespace Util.ORM
{
    public class DBHelper : IDisposable
    {
        /// <summary>
        /// 数据库通用驱动连接
        /// </summary>
        public IDbConnection DbConnection { get; private init; }
        public DBHelper(IDbConnection dbConnection) => DbConnection = dbConnection;
        /// <summary>
        /// 命令执行超时时间，单位 seconds
        /// </summary>
        public int CommandTimeout { get; set; } = 30;
        /// <summary>
        /// 事务
        /// </summary>
        public IDbTransaction? DbTransaction { get; private set; }
        /// <summary>
        /// 是否保持IDbConnection长连接开启
        /// <para>为true时，每次数据库操作后将不会关闭连接</para>
        /// </summary>
        public bool IsLongConnection { get; set; }

        #region ExecuteNonQuery
        public int ExecuteNonQuery(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters)
        {
            DbConnection.WhenCloseOpen();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            var output = cmd.ExecuteNonQuery();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters)
        {
            await DbConnection.WhenCloseOpenAsync();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            var output = await cmd.ExecuteNonQueryAsync();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        #endregion

        #region ExecuteScalar
        public object? ExecuteScalar(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters)
        {
            DbConnection.WhenCloseOpen();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            var output = cmd.ExecuteScalar();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        public async Task<object?> ExecuteScalarAsync(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters)
        {
            await DbConnection.WhenCloseOpenAsync();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            var output = await cmd.ExecuteScalarAsync();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        #endregion

        #region ExecuteReader
        public DataSet ExecuteReader(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters, CommandBehavior behavior = CommandBehavior.Default)
        {
            DbConnection.WhenCloseOpen();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            using var reader = cmd.ExecuteReader(behavior);
            var output = reader.ToDataSet();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        public async Task<DataSet> ExecuteReaderAsync(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters, CommandBehavior behavior = CommandBehavior.Default)
        {
            await DbConnection.WhenCloseOpenAsync();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            using var reader = await cmd.ExecuteReaderAsync(behavior);
            var output = reader.ToDataSet();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        public List<TMapper> ExecuteReader<TMapper>(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters, CommandBehavior behavior = CommandBehavior.Default) where TMapper : class, new()
        {
            DbConnection.WhenCloseOpen();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            using var reader = cmd.ExecuteReader(behavior);
            var output = reader.ToList<TMapper>();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        public async Task<List<TMapper>> ExecuteReaderAsync<TMapper>(CommandType commandType, string commandText, IEnumerable<IDbDataParameter>? parameters, CommandBehavior behavior = CommandBehavior.Default) where TMapper : class, new()
        {
            await DbConnection.WhenCloseOpenAsync();
            using var cmd = DbConnection.BuilderCommand(commandType, commandText, parameters, CommandTimeout, DbTransaction);
            using var reader = await cmd.ExecuteReaderAsync(behavior);
            var output = reader.ToList<TMapper>();
            if (IsLongConnection is false) DbConnection.Close();
            return output;
        }
        #endregion

        #region dispose

        bool _isDisposed;
        void Dispose()
        {
            if (_isDisposed) return;
            else
            {
                DbConnection.Dispose();
                _isDisposed = true;
            }
        }
        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this); ///告诉GC不需要再次调用
        }
        ~DBHelper()
        {
            Dispose();
        }

        #endregion
    }

}
