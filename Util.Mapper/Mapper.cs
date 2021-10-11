﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Util.Reflection.Expressions;
using System.Text.Json;
using System.Text;
using Util.Reflection.Expressions.Abstractions;
using Util.Reflection.Types;

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
                    dicList.BlockMethod("Add", BuildDictionary(source));
                });
                return dicList.BuildDelegate<Func<IEnumerable<TSource>, List<Dictionary<string, string>>>>();
            }
        }
        static Var BuildDictionary(CommonValueExpression source)
        {            
            Var mapper = Expr.New<Dictionary<string, string>>();
            var sourceProperties = source.Type.GetProperties();
            //var jsonConvert = Expr.Static(typeof(JsonSerializer));
            var jsonConvert = Expr.Static(typeof(Mapper));
            foreach (var sp in sourceProperties)
            {
                var spt = sp.PropertyType;
                if (spt.IsValueType) mapper[$"[{sp.Name}]"] = source[sp.Name].Method("ToString");
                else if (spt == typeof(string)) mapper[$"[{sp.Name}]"] = source[sp.Name];
                //else mapper[$"[{sp.Name}]"] = jsonConvert.Method("Serialize", source[sp.Name]);
                else mapper[$"[{sp.Name}]"] = jsonConvert.Method("ToJson", source[sp.Name]);
            }
            return mapper;
        }
        #endregion

        #region ToJson 序列化/反序列化
        public static string ToJson<TSource>(TSource input)
        {
            string key = $"[{typeof(TSource).FullName}]ToJson";
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
                var instance = Expr.BlockParam(typeof(TSource));
                Var stringBuilder = Expr.New<StringBuilder>();
                BuildJson(instance, stringBuilder);
                stringBuilder.BlockMethod("ToString");
                return stringBuilder.BuildDefaultDelegate<Func<TSource, string>>();
            }           
        }
        static void BuildJson(CommonValueExpression instance, Var stringBuilder)
        {
            string append = nameof(StringBuilder.Append);
            var descriptor = new TypeDescriptor(instance.Type);
            if (descriptor.TypeEnum == TypeEnum.ValueType)
            {
                if(descriptor.TypeDetailEnum==TypeDetailEnum.Bool)
                {
                    stringBuilder.BlockMethod(append, instance.Method("ToString").Method("ToLower"));
                }
                else if(descriptor.TypeDetailEnum == TypeDetailEnum.DateTime)
                {
                    stringBuilder.BlockMethod(append, "\"" + instance.Method("ToString") + "\"");
                }
                else
                {
                    stringBuilder.BlockMethod(append, instance);
                }              
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

        public static TSource ToClass<TSource>(string input) where TSource : class,new()
        {
            string key = $"[{typeof(TSource).Name}]ToClass";
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
                var descriptor = new TypeDescriptor(typeof(TSource));
                var json = Expr.BlockParam(typeof(string));
                var bytes = Expr.Static<Encoding>().MemberQuery("UTF8").Method("GetBytes", json);
                var options = Expr.New(typeof(JsonReaderOptions));
                options.InitMember(Expr.Constant(true, "AllowTrailingCommas"), Expr.Constant(JsonCommentHandling.Skip, "CommentHandling"));
                Var jsonReader = Expr.New(typeof(Utf8JsonReader), bytes.Convert(typeof(ReadOnlySpan<byte>)), options);
                Var instance = BuildClass(jsonReader, descriptor);
                return instance.BuildDelegate<Func<string, TSource>>();
            }
        }
        static Var BuildClass(Var jsonReader, TypeDescriptor descriptor)
        {
            Var instance = Expr.Constant(null, descriptor.Type);
            Expr.While(jsonReader.Method("Read"), (c, r) =>
            {
                if(descriptor.TypeEnum!= TypeEnum.Array)
                {
                    Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.PropertyName), () =>
                    {
                        Var propertyName = jsonReader.Method("GetString");
                        foreach (var ps in descriptor.Properties)
                        {
                            Expr.IfThen(propertyName == Expr.Constant(ps.Name), () =>
                            {
                                if (ps.TypeEnum == TypeEnum.ValueType || ps.TypeEnum == TypeEnum.String)
                                {
                                    jsonReader.BlockMethod("Read");
                                    if (ps.TypeDetailEnum == TypeDetailEnum.String)
                                    {
                                        Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.String), () =>
                                        {
                                            instance[ps.Name] = jsonReader.Method("GetString");
                                        });
                                    }
                                    else if (ps.TypeDetailEnum == TypeDetailEnum.Bool)
                                    {
                                        Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.True) || jsonReader["TokenType"] == Expr.Constant(JsonTokenType.False), () =>
                                        {
                                            instance[ps.Name] = jsonReader.Method("GetBoolean");
                                        });
                                    }
                                    else if (ps.TypeDetailEnum == TypeDetailEnum.Char)
                                    {
                                        Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.String), () =>
                                        {
                                            instance[ps.Name] = jsonReader.Method("GetString")[0];
                                        });
                                    }
                                    else if (ps.TypeDetailEnum == TypeDetailEnum.DateTime)
                                    {
                                        Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.String), () =>
                                        {
                                            instance[ps.Name] = Expr.Static(typeof(Convert)).Method("ToDateTime", jsonReader.Method("GetString"));
                                        });
                                    }
                                    else
                                    {
                                        Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.Number), () =>
                                        {
                                            if (ps.TypeDetailEnum == TypeDetailEnum.Int) instance[ps.Name] = jsonReader.Method("GetInt32");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Uint) instance[ps.Name] = jsonReader.Method("GetUInt32");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Short) instance[ps.Name] = jsonReader.Method("GetInt16");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Ushort) instance[ps.Name] = jsonReader.Method("GetUInt16");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Byte) instance[ps.Name] = jsonReader.Method("GetByte");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Sbyte) instance[ps.Name] = jsonReader.Method("GetSByte");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Long) instance[ps.Name] = jsonReader.Method("GetInt64");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Ulong) instance[ps.Name] = jsonReader.Method("GetUInt64");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Decimal) instance[ps.Name] = jsonReader.Method("GetDecimal");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Double) instance[ps.Name] = jsonReader.Method("GetDouble");
                                            else if (ps.TypeDetailEnum == TypeDetailEnum.Float) instance[ps.Name] = jsonReader.Method("GetSingle");
                                        });
                                    }
                                }
                                else
                                {
                                    instance[ps.Name] = BuildClass(jsonReader, ps);
                                }
                                c();
                            });
                        }
                        jsonReader.BlockMethod("Skip");
                        c();
                    });

                    Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.StartObject), () =>
                    {
                        if (descriptor.TypeEnum == TypeEnum.Object)
                        {
                            instance.Assgin(Expr.New(descriptor.Type));
                        }
                        else r();
                    });

                    Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.EndObject), () =>
                    {
                        r();
                    });
                }
                else
                {

                    Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.StartArray), () =>
                    {
                        if (descriptor.TypeEnum == TypeEnum.Array)
                        {
                            instance.Assgin(Expr.New(descriptor.Type));
                            Expr.While(jsonReader["TokenType"] != Expr.Constant(JsonTokenType.EndArray), (_, _) =>
                            {
                                instance.BlockMethod("Add", BuildClass(jsonReader, descriptor.GenericTypeArguments[0]));
                            });
                            instance.BlockMethod("RemoveAt", instance["Count"]-1);
                        }
                        else r();
                    });

                    Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.EndArray), () =>
                    {
                        r();
                    });
                }
            });
            return instance;
        }

        static Var BuildClassNew(Var jsonReader, TypeDescriptor descriptor)
        {
            Var instance = Expr.Constant(null, descriptor.Type);
            Expr.While(jsonReader.Method("Read"), (c, r) =>
            {
                if (descriptor.TypeEnum == TypeEnum.Object)
                {

                }
                else if(descriptor.TypeEnum == TypeEnum.Array)
                {
                    Expr.IfThen(jsonReader["TokenType"] == Expr.Constant(JsonTokenType.StartArray), () =>
                    {
                        instance.Assgin(Expr.New(descriptor.Type));
                        Expr.While(jsonReader["TokenType"] != Expr.Constant(JsonTokenType.EndArray), (_, _) =>
                        {

                            instance.BlockMethod("Add", BuildClass(jsonReader, descriptor.GenericTypeArguments[0]));
                        });
                    });
                }
                r();
            });
            return instance;
        }
        #endregion
    }
}
