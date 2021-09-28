using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Util.Reflection.Expressions;
using Util.Reflection.Expressions.Abstractions;

namespace Util.Extensions.Reflection.Types
{
    public static class TypeExtensions
    {
        public static MemberInfo? MemberQueryNew(this Type type, string memberName, IEnumerable<Type>? methodParamTypes,IEnumerable<Type>? genericParamTypes)
        {
            var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (members.Count() >0)
            {
                foreach (var member in members.Where(m => m.Name == memberName))
                {
                    if (member is PropertyInfo property) return property.PropertyType;
                    else if (member is FieldInfo field) return field.FieldType;
                    if (member is MethodInfo method)
                    {
                        if (method.ContainsGenericParameters)
                        {
                            if (genericParamTypes is null) return null;
                            var sourceGenericParameters = method.GetGenericArguments().ToList();
                            var compareGenericParameters = genericParamTypes.ToList();
                            if (CheckTypeIsSame(sourceGenericParameters, compareGenericParameters) == false) continue;
                            method.MakeGenericMethod(genericParamTypes!.ToArray());

                            var sourceParameterInfos = method.GetParameters();
                            var sourceParameterTypes = sourceParameterInfos.Select(sp => sp.ParameterType).ToList();
                            if (methodParamTypes is null)
                            {
                                if (sourceParameterTypes.Count > 0) continue;
                                else return method;
                            }
                            else
                            {
                                var compareParameters = methodParamTypes.ToList();
                                if (CheckTypeIsSame(sourceParameterTypes, compareParameters) == false) continue;
                                else return method;
                            }
                        }
                        else
                        {
                            var sourceParameterInfos = method.GetParameters();
                            var sourceParameterTypes = sourceParameterInfos.Select(sp => sp.ParameterType).ToList();
                            if (methodParamTypes is null)
                            {
                                if (sourceParameterTypes.Count > 0) continue;
                                else return method;
                            }
                            else
                            {
                                var compareParameters = methodParamTypes.ToList();
                                if (CheckTypeIsSame(sourceParameterTypes, compareParameters) == false) continue;
                                else return method;
                            }
                        }
                    }
                }
            }
          
            return null;

            bool CheckTypeIsSame(List<Type> source, List<Type> compare)
            {
                if (source.Count != compare.Count) return false;
                for (var i = 0; i < source.Count; i++)
                {
                    if (source[i] != compare[i]) return false;
                }
                return true;
            }
        }
        public static MemberInfo? MemberQuery(this Type type, string memberName, IEnumerable<Type>? methodParamTypes)
        {
            //var members = type.GetMembers();
            MemberInfo ? member = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (member is null)
            {
                member = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (member is null) member = type.GetMethod(memberName, methodParamTypes?.ToArray()??new Type[] { });
            }
            return member;
        }
        public static MemberInfo? MemberQuery(this Type type, string memberName, params Type[] methodParamTypes)
        {
            return type.MemberQuery(memberName, (IEnumerable<Type>)methodParamTypes);
        }
    }
}
