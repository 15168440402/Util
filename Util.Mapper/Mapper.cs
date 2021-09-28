using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Util.Reflection.Expressions;
using System.Text.Json;
using System.Text;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Mapper
{
    public static class Mapper
    {
        public static readonly ConcurrentDictionary<string, Delegate> _map;
        static Mapper()
        {
            _map = new ConcurrentDictionary<string, Delegate>();
        }

        #region To
        public static TMapper To<TSource, TMapper>(TSource input,bool shallowCopy=true) where TMapper : new()
        {
            string key = $"[{typeof(TSource).Name}]To[{typeof(TMapper).Name}]With[{shallowCopy}]";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<TSource, TMapper>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToDelegate();
                _map.TryAdd(key, func);
                return func(input);
            }

            Func<TSource, TMapper> BuildToDelegate()
            {
                Var mapper = BuildExpr(Expr.Param<TSource>(), typeof(TMapper));
                return mapper.BuildDelegate<Func<TSource, TMapper>>();
            }
        }        
        public static IEnumerable<TMapper> ToList<TSource, TMapper>(IEnumerable<TSource> input) where TMapper : new()
        {
            string key = $"[{typeof(TSource).Name}]ToList[{typeof(TMapper).Name}]";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<IEnumerable<TSource>, IEnumerable<TMapper>>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToListDelegate();
                _map.TryAdd(key, func);
                return func(input);
            }

            Func<IEnumerable<TSource>, List<TMapper>> BuildToListDelegate()
            {
                Var sourList = Expr.Param<IEnumerable<TSource>>();
                Var mapperList = Expr.New<List<TMapper>>();
                Expr.Foreach(sourList, (source, c, r) =>
                {
                    mapperList.Method("Add", BuildExpr(source, typeof(TMapper)));
                });
                return mapperList.BuildDelegate<Func<IEnumerable<TSource>, List<TMapper>>>();
            }
        }
        static Var BuildExpr(CommonValueExpression source, Type mapperType)
        {
            Var mapper = Expr.New(mapperType);
            var sourceProperties = source.Type.GetProperties();
            var mapperProperties = mapperType.GetProperties();
            foreach (var mp in mapperProperties)
            {
                var sp = sourceProperties.FirstOrDefault(sp => sp.Name == mp.Name);
                if (sp is null) continue;
                var st = sp.PropertyType;
                var mt = mp.PropertyType;
                var spn = sp.Name;
                if (st.IsValueType || st == typeof(string))
                {
                    if (st == mt) mapper[spn] = source[spn];
                    else mapper[spn] = source[spn].Convert(mt);
                }
                else
                {
                    Expr.IfThen(source[spn] != Expr.Constant(null, st), () =>
                    {
                        mapper[mp.Name] = BuildExpr(source[spn], mt);
                    });
                }
            }
            return mapper;
        }
        #endregion

        #region ToCopy
        public static T ToCopy<T>(T input, bool shallowCopy = true)
        {
            string key = $"[{typeof(T).Name}]ToCopyWith[{shallowCopy}]";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<T, T>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToCopyDelegate(shallowCopy);
                _map.TryAdd(key, func);
                return func(input);
            }

            Func<T, T> BuildToCopyDelegate(bool shallowCopy)
            {
                Var source = Expr.Param<T>();
                Var copy = Copy(source, shallowCopy);
                return copy.BuildDelegate<Func<T, T>>();
            }
        }      
        public static List<T> ToCopyList<T>(IEnumerable<T> input, bool shallowCopy = true)
        {
            string key = $"[{typeof(T).Name}]ToCopyList[{shallowCopy}]";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<IEnumerable<T>, List<T>>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToCopyListDelegate(shallowCopy);
                _map.TryAdd(key, func);
                return func(input);
            }

            Func<IEnumerable<T>, List<T>> BuildToCopyListDelegate(bool shallowCopy)
            {
                Var sourList = Expr.Param<IEnumerable<T>>();
                Var copyList = Expr.New<List<T>>();
                Expr.Foreach(sourList, (source, c, r) =>
                {
                    copyList.Method("Add", Copy(source, shallowCopy));
                });
                return copyList.BuildDelegate<Func<IEnumerable<T>, List<T>>>();
            }
        }
        static Var Copy(CommonValueExpression source, bool shallowCopy)
        {
            Var copy = Expr.New(source.Type);
            var properties = source.Type.GetProperties();
            if (shallowCopy == true)
            {
                foreach (var p in properties)
                {
                    var name = p.Name;
                    copy[name] = source[name];
                }
            }
            else
            {
                foreach (var p in properties)
                {
                    var type = p.PropertyType;
                    var name = p.Name;
                    if (type.IsValueType || type == typeof(string))
                    {
                        copy[name] = source[name];
                    }
                    else
                    {
                        Expr.IfThen(copy[name] != Expr.Constant(null,type), () =>
                        {
                            Copy(copy[name], false);
                        });
                    }
                }
            }
            return copy;
        }
        #endregion 

        #region ToDictionary
        public static Dictionary<string, string> ToDictionary<TSource>(TSource input)
        {
            string key = $"[{typeof(TSource).Name}]ToDictionary[{typeof(Dictionary<string, string>).Name}]";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<TSource, Dictionary<string, string>>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToDictionaryDelegate();
                _map.TryAdd(key, func);
                return func(input);
            }

            Func<TSource, Dictionary<string, string>> BuildToDictionaryDelegate()
            {
                Var source = Expr.Param<TSource>();
                Var dic = BuildDictionary(source);
                return dic.BuildDelegate<Func<TSource, Dictionary<string, string>>>();
            }
        }
        public static List<Dictionary<string, string>> ToListDictionary<TSource>(IEnumerable<TSource> input)
        {
            string key = $"[{typeof(TSource).Name}]ToListDictionary[{typeof(Dictionary<string, string>).Name}]";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<IEnumerable<TSource>, List<Dictionary<string, string>>>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToListDictionaryDelegate();
                _map.TryAdd(key, func);
                return func(input);
            }

            Func<IEnumerable<TSource>, List<Dictionary<string, string>>> BuildToListDictionaryDelegate()
            {
                Var sourList = Expr.Param<IEnumerable<TSource>>();
                Var dicList = Expr.New<List<Dictionary<string, string>>>();

                Expr.Foreach(sourList, (source, c, r) =>
                {
                    dicList.Method("Add", BuildDictionary(source));
                });
                return dicList.BuildDelegate<Func<IEnumerable<TSource>, List<Dictionary<string, string>>>>();
            }
        }
        static Var BuildDictionary(CommonValueExpression source)
        {            
            Var mapper = Expr.New<Dictionary<string, string>>();
            var sourceProperties = source.Type.GetProperties();
            var jsonConvert = Expr.Static(typeof(JsonSerializer));
            foreach (var sp in sourceProperties)
            {
                var spt = sp.PropertyType;
                if (spt.IsValueType) mapper[$"[{sp.Name}]"] = source[sp.Name].Method("ToString");//source[sp.Name].Convert<string>();
                else if (spt == typeof(string)) mapper[$"[{sp.Name}]"] = source[sp.Name];
                else mapper[$"[{sp.Name}]"] = jsonConvert.Method("Serialize", source[sp.Name]);
            }
            return mapper;
        }
        #endregion

        #region ToJson 序列化/反序列化
        public static string ToJson<TSource>(TSource input)
        {
            string key = $"[{typeof(TSource).Name}]ToJson";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<TSource, string>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToJsonDelegate();
                _map.TryAdd(key, func);
                return func(input);
            }
            Func<TSource, string> BuildToJsonDelegate()
            {
                Var instance = Expr.Param<TSource>();
                Var stringBuilder = Expr.New<StringBuilder>();
                BuildJson(instance, stringBuilder);
                stringBuilder.BlockMethod("ToString");
                return stringBuilder.BuildDefaultDelegate<Func<TSource, string>>();
            }           
        }
        static void BuildJson(CommonValueExpression instance, Var stringBuilder)
        {
            string append = nameof(StringBuilder.Append);
            if (instance.Type.IsValueType)
            {
                stringBuilder.BlockMethod(append, instance.Method("ToString"));
            }
            else 
            {
                Expr.IfThenElse(instance == Expr.Constant(null, instance.Type), () =>
                {
                    stringBuilder.BlockMethod(append, "null");

                }, () =>
                {
                    if (instance.Type == typeof(string))
                    {
                        stringBuilder.BlockMethod(append, "\"" + instance + "\"");
                    }
                    else if (instance.Type.GetInterfaces().Any(t => t == typeof(System.Collections.IDictionary)))
                    {
                        stringBuilder.BlockMethod(append, "{");
                        Expr.Foreach(instance, (item, c, r) =>
                        {
                            var key = item["Key"];
                            if (key.Type != typeof(string))
                            {
                                throw new UtilException("Dictionary序列化只支持key为string类型");
                            }
                            stringBuilder.BlockMethod(append, $"\"" + key + "\":");
                            BuildJson(item["Value"], stringBuilder);                           
                            stringBuilder.BlockMethod(append, ",");
                        });
                        stringBuilder.BlockMethod("Remove", stringBuilder["Length"] - 1, 1);
                        stringBuilder.BlockMethod(append, "}");
                    }
                    else if (instance.Type.GetInterfaces().Any(t => t == typeof(System.Collections.IEnumerable)))
                    {
                        stringBuilder.BlockMethod(append, "[");
                        Expr.Foreach(instance, (item, c, r) =>
                        {
                            BuildJson(item, stringBuilder);
                            stringBuilder.BlockMethod(append, ",");
                        });
                        stringBuilder.BlockMethod("Remove", stringBuilder["Length"] - 1, 1);
                        stringBuilder.BlockMethod(append, "]");
                    }
                    else
                    {
                        stringBuilder.BlockMethod(append, "{");
                        var sourceProperties = instance.Type.GetProperties();
                        foreach (var st in sourceProperties)
                        {
                            stringBuilder.BlockMethod(append, $"\"{st.Name}\":");
                            BuildJson(instance[st.Name], stringBuilder);
                            stringBuilder.BlockMethod(append, ",");
                        }
                        stringBuilder.BlockMethod("Remove", stringBuilder["Length"] - 1, 1);
                        stringBuilder.BlockMethod(append, "}");
                    }
                });
            }
        }

        public static TSource ToClass<TSource>(string input)
        {
            string key = $"[{typeof(TSource).Name}]ToJson";
            if (_map.TryGetValue(key, out var @delegate))
            {
                var func = (Func<string, TSource>)@delegate;
                return func(input);
            }
            else
            {
                var func = BuildToClassDelegate();
                _map.TryAdd(key, func);
                return func(input);
            }
            Func<string, TSource> BuildToClassDelegate()
            {
                return default!;
            }
        }
        static void BuildClass(string json)
        {

        }
        #endregion
    }
}
