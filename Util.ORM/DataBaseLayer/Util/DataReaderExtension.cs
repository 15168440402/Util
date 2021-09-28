using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Util.ORM.Attributes;
using Util.Reflection.Expressions;

namespace Util.ORM.Util
{
    internal static class DataReaderExtension
    {
        static readonly ConcurrentDictionary<string, Delegate> _map;
        static DataReaderExtension()
        {
            _map = new ConcurrentDictionary<string, Delegate>();
        }
        public static DataTable ToDataTable(this IDataReader reader)
        {
            DataTable dt = new DataTable();
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                DataColumn dc = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                dt.Columns.Add(dc);
            }
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < fieldCount; i++)
                {
                    dr[i] = reader[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static DataSet ToDataSet(this IDataReader reader)
        {
            DataSet ds = new DataSet();
            do
            {
                ds.Tables.Add(reader.ToDataTable());
            } while (reader.NextResult());

            return ds;
        }
        public static List<TMapper> ToList<TMapper>(this IDataReader reader) where TMapper : class, new()
        {
            var func = QueryToListDelegate<TMapper>(reader);
            return func(reader);
        }
        static Func<IDataReader, List<TMapper>> QueryToListDelegate<TMapper>(IDataReader reader)
        {
            Type mapperType = typeof(TMapper);
            if (_map.TryGetValue(mapperType.Name, out var func)) return (Func<IDataReader, List<TMapper>>)func;
            else
            {
                var entityInfos = new List<(string, int, Type)>();
                var properties = mapperType.GetProperties();
                foreach (var p in properties)
                {
                    var notMap = p.GetCustomAttribute<NotMapperAttribute>();
                    if (notMap is not null && notMap.WhenSelect is true) continue;
                    var column = p.GetCustomAttribute<ColumnAttribute>();
                    string name = column?.Name ?? p.Name;
                    int index = reader.GetOrdinal(name);
                    entityInfos.Add((name, index, p.PropertyType));
                }
                return (Func<IDataReader, List<TMapper>>)_map.GetOrAdd(mapperType.Name, BuildDataReaderToListDelegate<TMapper>(entityInfos));
            }           
        }
        static Func<IDataReader, List<TMapper>> BuildDataReaderToListDelegate<TMapper>(List<(string, int, Type)> entityInfos)
        {
            Var ls = Expr.New<List<TMapper>>();
            Var reader = Expr.Param<IDataReader>();
            Var record = reader.Convert<IDataRecord>();
            Expr.While(reader.Method("Read"), (_, _) =>
            {
                Var mapper = Expr.New<TMapper>();
                foreach (var (name, index, type) in entityInfos)
                {
                    Expr.IfThen(record.Method("IsDBNull",index) == Expr.Constant(false), () =>
                    {
                        mapper[name] = record[$"[{index}]"].Convert(type);
                    });
                }
                ls.BlockMethod("Add", mapper);
            });
            return ls.BuildDelegate<Func<IDataReader, List<TMapper>>>();
        }
    }
}
