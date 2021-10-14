using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Util.Common;

namespace Util.Reflection.Types
{
    public partial class TypeDescriptor
    {
        public TypeDescriptor(Type type, string name = "") => (Type, Name) = (type, name);
        public string Name { get; init; }
        public Type Type { get; init; }

        TypeEnum? _typeEnum;
        public TypeEnum? TypeEnum => _typeEnum ?? (_typeEnum = TypeDescriptorHelper.GetTypeEnum(Type));
        TypeDetailEnum? _typeDetailEnum;
        public TypeDetailEnum? TypeDetailEnum => _typeDetailEnum ?? (_typeDetailEnum = TypeDescriptorHelper.GetTypeDetailEnum(Type));

        List<TypeDescriptor>? _properties;
        public List<TypeDescriptor> Properties => _properties ?? (TypeDescriptorHelper.GetProperties(Type));
        public TypeDescriptor? QueryProperty(string name)
        {
            return Properties.FirstOrDefault(p=>p.Name== name);
        }

        public List<TypeDescriptor> GenericTypeArguments=> Type.GenericTypeArguments.Select(gt => new TypeDescriptor(gt)).ToList();
    }
    
    public partial class TypeDescriptor
    {
        public static MemberInfo[] GetMembers(Type type)
        {
            return type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }
        public static PropertyInfo[] GetPropertys(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
        /// <summary>
        /// 获取指定类型的第一个符合指定条件的成员
        /// <para>注解：成员包含字段、属性、索引器、方法等</para>
        /// </summary>
        /// <param name="type">class/struct的<see cref="Type"/></param>
        /// <param name="memberName">成员名称</param>
        /// <param name="methodParamTypes">如果查询的成员是方法，需要指定方法入参的<see cref="Type"/></param>
        /// <param name="genericParamTypes">如果是泛型方法且需要显示指定泛型类型的，需要提供泛型<see cref="Type"/></param>
        /// <returns></returns>
        public static MemberDescriptor? QueryMember(Type type, string memberName, IEnumerable<Type>? methodParamTypes = null, IEnumerable<Type>? genericParamTypes = null)
        {
            var members = GetMembers(type).Where(m => m.Name == memberName);
            if (members.Count() == 0) return null;
            foreach (var member in members)
            {
                if (member is PropertyInfo property) return new MemberDescriptor(property);
                else if (member is FieldInfo field) return new MemberDescriptor(field);
                else if (member is MethodInfo method)
                {
                    var memberDescriptor= EqualMethod(method,methodParamTypes,genericParamTypes);
                    if (memberDescriptor is not null) return memberDescriptor;
                }
            }
            return null;
        }

        private static MemberDescriptor? EqualMethod(MethodInfo method, IEnumerable<Type>? compareParameTypes)
        {
            var parameterInfos = method.GetParameters().ToList();
            if (compareParameTypes is null)//方法无参情况下的比较
            {
                if (parameterInfos.Count == 0) return new MemberDescriptor(method);
                else if (parameterInfos.Where(p => p.HasDefaultValue is false).Count() == 0)
                {
                    var extraParamter = parameterInfos.Select(p => (p.DefaultValue,p.ParameterType)).ToList();
                    return new MemberDescriptor(method, extraParamter);
                }
                else return null;
            }
            else//方法有参情况下的比较
            {
                var parameterInfoEnumerator = parameterInfos.GetEnumerator();
                var compareParameTypeEnumerator = compareParameTypes.GetEnumerator();
                MemberDescriptor? output = default;
                while (parameterInfoEnumerator.MoveNext())
                {
                    if (compareParameTypeEnumerator.MoveNext())
                    {
                        if (compareParameTypeEnumerator.Current.Equals(parameterInfoEnumerator.Current.ParameterType) is false)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (parameterInfoEnumerator.Current.HasDefaultValue)
                        {
                            if (output is null) output = new MemberDescriptor(method, new List<(object?, Type type)>() { (parameterInfoEnumerator.Current.DefaultValue, parameterInfoEnumerator.Current.ParameterType) });
                            else output.ExtraParameters!.Add((parameterInfoEnumerator.Current.DefaultValue, parameterInfoEnumerator.Current.ParameterType));
                        }
                        else return null;
                    }
                }
                if (compareParameTypeEnumerator.MoveNext()) return null;
                if(output is null)output = new MemberDescriptor(method);
                return output;
            }
        }
        private static MemberDescriptor? EqualMethod(MethodInfo method, IEnumerable<Type>? compareParameTypes, IEnumerable<Type>? compareGenericParamTypes)
        {
            var genericArgumentTypes = method.GetGenericArguments();
            if (method.ContainsGenericParameters)//如果是泛型方法，则需要先指定泛型类型
            {
                if (compareGenericParamTypes is null)//需要检验此方法是不是不需要显示指定泛型类型
                {
                    if (compareParameTypes is not null)
                    {
                        var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
                        var genericArgumentCount = genericArgumentTypes.Count();
                        if (genericArgumentCount == paramTypes.Where(t=>t.IsGenericParameter).Count())
                        {
                            var paramTypeEnumerator = paramTypes.GetEnumerator();
                            var compareParameTypeEnumerator = compareParameTypes.GetEnumerator();
                            var makeGenericTypes = new List<Type>();
                            while(paramTypeEnumerator.MoveNext())
                            {
                                if (compareParameTypeEnumerator.MoveNext())
                                {
                                    if (paramTypeEnumerator.Current.IsGenericParameter) makeGenericTypes.Add(compareParameTypeEnumerator.Current);
                                }
                                else if (makeGenericTypes.Count != genericArgumentCount) return null;
                            }
                            method=method.MakeGenericMethod(makeGenericTypes.ToArray());
                        }
                        else return null;
                    }
                    else return null;
                }
                else if (genericArgumentTypes.Count() == compareGenericParamTypes.Count())
                {
                    method=method.MakeGenericMethod(compareGenericParamTypes.ToArray());
                }
                else return null;
            }
            return EqualMethod(method, compareParameTypes);
        }
       
    }
    public class MemberDescriptor
    {
        public MemberDescriptor(MemberInfo member, List<(object?, Type type)>? extraParameters = null) => (Member, ExtraParameters) = (member, extraParameters);
        public MemberInfo Member {get;set;}
        public List<(object?,Type type)>? ExtraParameters { get; set; }
        private Type? _memberType;
        public Type MemberType => _memberType ?? (_memberType= GetMemberType());
        private Type GetMemberType()
        {
            if (Member is PropertyInfo member) return member.PropertyType;
            if (Member is FieldInfo field) return field.FieldType;
            if (Member is MethodInfo method2) return method2.ReturnType;
            if (Member is MethodInfo method)
            {
                var memberType = default(Type);
                var genericTypes = method.GetParameters().Select(p=>p.ParameterType).ToList();
                if (method.ReturnType == typeof(void))
                {
                    var paramCount = genericTypes.Count;
                    if (paramCount == 0) memberType = typeof(Action);
                    else if (paramCount == 1) memberType = typeof(Action<>);
                    else if (paramCount == 2) memberType = typeof(Action<,>);
                    else if (paramCount == 3) memberType = typeof(Action<,,>);
                    else if (paramCount == 4) memberType = typeof(Action<,,,>);
                    else if (paramCount == 5) memberType = typeof(Action<,,,,>);
                    else if (paramCount == 6) memberType = typeof(Action<,,,,,>);
                    else if (paramCount == 7) memberType = typeof(Action<,,,,,,>);
                    else if (paramCount == 8) memberType = typeof(Action<,,,,,,,>);
                    else if (paramCount == 9) memberType = typeof(Action<,,,,,,,,>);
                }
                else
                {
                    genericTypes.Add(method.ReturnType);
                    var paramCount = genericTypes.Count;
                    if (paramCount == 1) memberType = typeof(Func<>);
                    else if (paramCount == 2) memberType = typeof(Func<,>);
                    else if (paramCount == 3) memberType = typeof(Func<,,>);
                    else if (paramCount == 4) memberType = typeof(Func<,,,>);
                    else if (paramCount == 5) memberType = typeof(Func<,,,,>);
                    else if (paramCount == 6) memberType = typeof(Func<,,,,,>);
                    else if (paramCount == 7) memberType = typeof(Func<,,,,,,>);
                    else if (paramCount == 8) memberType = typeof(Func<,,,,,,,>);
                    else if (paramCount == 9) memberType = typeof(Func<,,,,,,,,>);
                    else if (paramCount == 9) memberType = typeof(Func<,,,,,,,,,>);
                }
                if (memberType is null) throw new Exception("最大支持解析9个参数的方法");
                memberType = memberType.MakeGenericType(genericTypes.ToArray());
                return memberType;
            }
            throw new UtilException($"无法解析成员{Member.Name},此成员不属于字段、属性、方法");
        }
    }

    public enum TypeEnum
    {
        ValueType,
        String,
        Object,
        Array
    }
    public enum TypeDetailEnum
    {      
        Int,
        Uint,
        Short,
        Ushort,
        Long,
        Ulong,
        Byte,
        Sbyte,
        Nint,
        Nuint,
        Float,
        Double,
        Decimal,
        Bool,
        Char,
        String,
        DateTime,
        List,
        Array,
        Dictionary
    }
   
    static class TypeDescriptorHelper
    {
        internal static TypeEnum? GetTypeEnum(Type type)
        {
            if (type.IsValueType) return TypeEnum.ValueType;
            else if (type == typeof(string)) return TypeEnum.String;
            else if (type.GetInterfaces().Any(gt => gt == typeof(System.Collections.IEnumerable))) return TypeEnum.Array;
            else return TypeEnum.Object;
        }
        internal static TypeDetailEnum? GetTypeDetailEnum(Type type)
        {
            if (type.IsValueType)
            {
                if (type == typeof(int)) return TypeDetailEnum.Int;
                else if (type == typeof(uint)) return TypeDetailEnum.Uint;
                else if (type == typeof(short)) return TypeDetailEnum.Short;
                else if (type == typeof(ushort)) return TypeDetailEnum.Ushort;
                else if (type == typeof(byte)) return TypeDetailEnum.Byte;
                else if (type == typeof(sbyte)) return TypeDetailEnum.Sbyte;
                else if (type == typeof(long)) return TypeDetailEnum.Long;
                else if (type == typeof(ulong)) return TypeDetailEnum.Ulong;
                else if (type == typeof(nint)) return TypeDetailEnum.Nint;
                else if (type == typeof(nuint)) return TypeDetailEnum.Nuint;
                else if (type == typeof(float)) return TypeDetailEnum.Float;
                else if (type == typeof(double)) return TypeDetailEnum.Double;
                else if (type == typeof(decimal)) return TypeDetailEnum.Decimal;
                else if (type == typeof(bool)) return TypeDetailEnum.Bool;
                else if (type == typeof(char)) return TypeDetailEnum.Char;
                else if (type == typeof(DateTime)) return TypeDetailEnum.DateTime;
                else return null;
            }
            else if (type == typeof(string)) return TypeDetailEnum.String;
            else if (type.BaseType == typeof(Array)) return TypeDetailEnum.Array;
            else if (type.GetInterfaces().Any(t => t == typeof(System.Collections.IDictionary))) return TypeDetailEnum.Dictionary;
            else if (type.GetInterfaces().Any(gt => gt == typeof(System.Collections.ICollection)) && type.GenericTypeArguments.Length == 1) return TypeDetailEnum.List;
            else return null;
           
        }
        internal static List<TypeDescriptor> GetProperties(Type type)
        {
            return TypeDescriptor.GetPropertys(type).Select(p => new TypeDescriptor(p.PropertyType, p.Name)).ToList();
        }
    }
}
