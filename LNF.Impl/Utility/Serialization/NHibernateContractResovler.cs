using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHibernate.Proxy;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LNF.Impl.Util.Serialization
{
    public class NHibernateContractResovler : DefaultContractResolver
    {
        private static readonly MemberInfo[] NHibernateProxyInterfaceMembers = typeof(INHibernateProxy).GetMembers();

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            int x = 0;
            if (objectType == typeof(DateTimeOffsetType))
            {
                x++;
            }
            var members = base.GetSerializableMembers(objectType);

            members.RemoveAll(memberInfo =>
                              (IsMemberPartOfNHibernateProxyInterface(memberInfo)) ||
                              (IsMemberDynamicProxyMixin(memberInfo)) ||
                              (IsMemberMarkedWithIgnoreAttribute(memberInfo, objectType)) ||
                              (IsMemberInheritedFromProxySuperclass(memberInfo, objectType)));

            var actualMemberInfos = new List<MemberInfo>();

            foreach (var memberInfo in members)
            {
                var infos = new MemberInfo[0];

                if (memberInfo.DeclaringType.BaseType != null)
                    infos = memberInfo.DeclaringType.BaseType.GetMember(memberInfo.Name);

                actualMemberInfos.Add(infos.Length == 0 ? memberInfo : infos[0]);
            }

            return actualMemberInfos;
        }

        private static bool IsMemberDynamicProxyMixin(MemberInfo memberInfo)
        {
            bool result =  memberInfo.Name == "__interceptors";
            return result;
        }

        private static bool IsMemberInheritedFromProxySuperclass(MemberInfo memberInfo, Type objectType)
        {
            bool result = memberInfo.DeclaringType.Assembly == typeof(INHibernateProxy).Assembly;
            return result;
        }

        private static bool IsMemberMarkedWithIgnoreAttribute(MemberInfo memberInfo, Type objectType)
        {
            var infos = typeof(INHibernateProxy).IsAssignableFrom(objectType)
                          ? objectType.BaseType.GetMember(memberInfo.Name)
                          : objectType.GetMember(memberInfo.Name);

            bool result = infos[0].GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0;
            return result;
        }

        private static bool IsMemberPartOfNHibernateProxyInterface(MemberInfo memberInfo)
        {
            bool result = Array.Exists(NHibernateProxyInterfaceMembers, mi => memberInfo.Name == mi.Name);
            return result;
        }
    }
}
