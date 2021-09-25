using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MessageCommunicator.TestGui
{
    public class DynamicMappingWithAliasSerializationBinder : DefaultSerializationBinder
    {
        private const string ASSEMBLY_ALIAS = "__alias";

        private ResolveTypeByAliasDelegate _typeResolver;

        public DynamicMappingWithAliasSerializationBinder(ResolveTypeByAliasDelegate typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            var aliasAttrib = serializedType.GetCustomAttribute<TypeAliasAttribute>();
            if (aliasAttrib == null)
            {
                throw new JsonSerializationException(
                    $"Unable to serialize type {serializedType.FullName} because dynamic mapping is only supported for types with {nameof(TypeAliasAttribute)}!");
            }

            if (serializedType.IsGenericType)
            {
                throw new JsonSerializationException(
                    $"Unable to serialize type {serializedType.FullName} because dynamic mapping of generic types is not supported!");
            }

            assemblyName = ASSEMBLY_ALIAS;
            typeName = aliasAttrib.AliasName;
        }

        public override Type BindToType(string? assemblyName, string typeName)
        {
            if (assemblyName != ASSEMBLY_ALIAS)
            {
                throw new JsonSerializationException($"Unable to load type {typeName} from assembly {assemblyName}: Dynamic resolving only available for alias names!");
            }

            var resolvedType = _typeResolver(typeName);
            if (resolvedType == null)
            {
                throw new JsonSerializationException($"Unable to load type {typeName} from assembly {assemblyName}!");
            }

            return resolvedType;
        }
    }
}
