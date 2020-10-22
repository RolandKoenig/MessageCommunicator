using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui
{
    public static class SerializationHelper
    {
        public static readonly JsonSerializer Serializer = new JsonSerializer();

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
