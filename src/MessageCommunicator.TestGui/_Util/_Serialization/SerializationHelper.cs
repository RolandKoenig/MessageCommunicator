using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui
{
    public static class SerializationHelper
    {
        public static readonly JsonSerializer Serializer;

        static SerializationHelper()
        {
            Dictionary<string, Type> typesByAlias = new Dictionary<string, Type>(16);
            foreach (var actType in Assembly.GetExecutingAssembly().GetTypes())
            {
                var aliasAttrib = actType.GetCustomAttribute<TypeAliasAttribute>();
                if(aliasAttrib == null){ continue; }

                typesByAlias[aliasAttrib.AliasName] = actType;
            }


            Serializer = new JsonSerializer();
            Serializer.TypeNameHandling = TypeNameHandling.Auto;
            Serializer.SerializationBinder = new DynamicMappingWithAliasSerializationBinder(
                (aliasName) =>
                {
                    if (typesByAlias.TryGetValue(aliasName, out var foundType))
                    {
                        return foundType;
                    }
                    return null;
                });
        }

        public static T? DeserializeFromStream<T>(Stream inStream)
            where T : class
        {
            using (var inJsonStream = new JsonTextReader(new StreamReader(inStream)))
            {
                return Serializer.Deserialize<T>(inJsonStream);
            }
        }

        public static void SerializeToStream<T>(Stream outStream, T valueToSerialize)
            where T : class
        {
            using (var outJsonStream = new JsonTextWriter(new StreamWriter(outStream)))
            {
                outJsonStream.Formatting = Formatting.Indented;
                outJsonStream.IndentChar = ' ';
                outJsonStream.Indentation = 4;

                Serializer.Serialize(outJsonStream, valueToSerialize);
            }
        }
    }
}
