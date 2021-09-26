using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FirLib.Core
{
    public static class FirLibTools
    {
        public static TResult? ReadPrivateMember<TResult, TSourceType>(TSourceType sourceObject, string memberName)
        {
            var fInfo = typeof(TSourceType).GetTypeInfo().GetField(memberName, BindingFlags.NonPublic | BindingFlags.Instance);
            if(fInfo == null)
            {
                throw new InvalidOperationException($"Filed {memberName} not found!");
            }
            return (TResult?)fInfo.GetValue(sourceObject);
        }
    }
}
