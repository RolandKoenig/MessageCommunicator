using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MessageCommunicator.TestGui
{
    public class ReleaseOverview
    {
        private const string URL = "https://www.rolandk.de/releases/message_communicator.json";
        
        public static async Task<ReleaseInformation?> GetLatestReleaseAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                
                // List data response.
                HttpResponseMessage response = await client.GetAsync(new Uri(URL));  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {

                    await using(var inStream = await response.Content.ReadAsStreamAsync())
                    using (var inStreamReader = new StreamReader(inStream))
                    using (var jsonReader = new JsonTextReader(inStreamReader))
                    {
                        dynamic jObject = JsonSerializer.CreateDefault().Deserialize(jsonReader!)!;
                        string version = jObject[0].version;
                        return new ReleaseInformation(
                            Version.Parse(version));
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}