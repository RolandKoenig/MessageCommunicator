using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using TcpCommunicator.TestGui.Data;

namespace TcpCommunicator.TestGui.Logic
{
    public class ConnectionProfileStore
    {
        private static ConnectionProfileStore s_current;

        private JsonSerializer _serializer;

        public static ConnectionProfileStore Current
        {
            get
            {
                if(s_current == null){ s_current = new ConnectionProfileStore(); }
                return s_current;
            }
        }

        public ConnectionProfileStore()
        {
            _serializer = new JsonSerializer();
        }

        public void StoreConnectionProfiles(IEnumerable<ConnectionProfile> profiles)
        {
            var connParams = new List<ConnectionParameters>(12);
            foreach (var actProfile in profiles)
            {
                connParams.Add(actProfile.Parameters);
            }

            try
            {
                var safePath = GetProfileSafePath();
                var safeDirectory = Path.GetDirectoryName(safePath);
                if (!Directory.Exists(safeDirectory)) { Directory.CreateDirectory(safeDirectory); }

                using(var writer = new JsonTextWriter(new StreamWriter(File.OpenWrite(safePath))))
                {
                    writer.Formatting = Formatting.Indented;
                    _serializer.Serialize(writer, connParams);
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
                    var loadedConnParameters = _serializer.Deserialize<List<ConnectionParameters>>(reader);
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
