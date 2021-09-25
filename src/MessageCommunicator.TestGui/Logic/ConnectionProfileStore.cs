using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MessageCommunicator.TestGui.Data;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui.Logic
{
    public class ConnectionProfileStore
    {
        public static ConnectionProfileStore Current { get; } = new ConnectionProfileStore();

        public void StoreConnectionProfiles(IEnumerable<IConnectionProfile> profiles)
        {
            var connParams = new List<ConnectionParameters>(12);
            foreach (var actProfile in profiles)
            {
                connParams.Add(actProfile.Parameters);
            }

            try
            {
                var safePath = GetProfileSafePath();
                var safeDirectory = Path.GetDirectoryName(safePath)!;
                if (!Directory.Exists(safeDirectory)) { Directory.CreateDirectory(safeDirectory); }

                using(var writer = new JsonTextWriter(new StreamWriter(new FileStream(safePath, FileMode.Create, FileAccess.Write))))
                {
                    writer.Formatting = Formatting.Indented;
                    SerializationHelper.Serializer.Serialize(writer, connParams);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public List<ConnectionProfile>? LoadConnectionProfiles(SynchronizationContext syncContext)
        {
            var profileSafePath = GetProfileSafePath();
            if (!File.Exists(profileSafePath)) { return null; }

            try
            {
                using (var reader = new JsonTextReader(new StreamReader(File.OpenRead(profileSafePath))))
                {
                    var loadedConnParameters = SerializationHelper.Serializer.Deserialize<List<ConnectionParameters>>(reader);
                    if (loadedConnParameters == null) { return null; }

                    var result = new List<ConnectionProfile>(loadedConnParameters.Count);
                    foreach (var actConnParameters in loadedConnParameters)
                    {
                        result.Add(new ConnectionProfile(syncContext, actConnParameters));
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetProfileSafePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.Combine("TcpCommunicator", "TcpCommunicator.json"));
        }
    }
}
