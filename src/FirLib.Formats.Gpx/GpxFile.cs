using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FirLib.Formats.Gpx.Metadata;

namespace FirLib.Formats.Gpx
{
    [XmlType("gpx")]
    public class GpxFile
    {
        private static List<Type>? s_extensionTypes;
        private static List<(string, string)>? s_extensionNamespaces;
        private static ConcurrentDictionary<GpxVersion, XmlSerializer> s_cachedSerializer;

        [XmlAttribute("version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("creator")]
        public string Creator { get; set; } = string.Empty;

        [XmlElement("metadata")]
        public GpxMetadata? Metadata { get; set; }

        [XmlElement("wpt")]
        public List<GpxWaypoint> Waypoints { get; } = new();

        [XmlElement("rte")]
        public List<GpxRoute> Routes { get; } = new();

        [XmlElement("trk")]
        public List<GpxTrack> Tracks { get; } = new();

        [XmlElement("extensions")]
        public GpxExtensions? Extensions { get; set; }

        [XmlNamespaceDeclarations]  
        public XmlSerializerNamespaces? Xmlns { get; set; }

        static GpxFile()
        {
            s_cachedSerializer = new ConcurrentDictionary<GpxVersion, XmlSerializer>();
        }

        public GpxTrack CreateAndAddDummyTrack(string name, params GpxWaypoint[] waypoints)
        {
            var gpxTrack = new GpxTrack();
            gpxTrack.Name = name;
            this.Tracks.Add(gpxTrack);

            var gpxTrackSegment = new GpxTrackSegment();
            gpxTrack.Segments.Add(gpxTrackSegment);

            gpxTrackSegment.Points.AddRange(waypoints);

            return gpxTrack;
        }

        public GpxRoute CreateAndAddDummyRoute(string name, params GpxWaypoint[] waypoints)
        {
            var gpxRoute = new GpxRoute();
            gpxRoute.Name = name;
            this.Routes.Add(gpxRoute);

            gpxRoute.RoutePoints.AddRange(waypoints);

            return gpxRoute;
        }

        public void EnsureNamespaceDeclarations()
        {
            if(s_extensionNamespaces != null)
            {
                this.Xmlns ??= new XmlSerializerNamespaces();
                for(var loop=0; loop<s_extensionNamespaces.Count; loop++)
                {
                    var actTuple = s_extensionNamespaces[loop];
                    this.Xmlns.Add(actTuple.Item1, actTuple.Item2);
                }
            }
        }

        public static void RegisterNamespace(string namespacePrefix, string namespaceUri)
        {
            s_extensionNamespaces ??= new List<(string, string)>();
            for(var loop=0; loop<s_extensionNamespaces.Count; loop++)
            {
                var actEntry = s_extensionNamespaces[loop];
                if (actEntry.Item1 == namespacePrefix)
                {
                    if (actEntry.Item2 != namespaceUri)
                    {
                        throw new GpxFileException(
                            $"Namespace with prefix {namespacePrefix} already registered for {actEntry.Item2}!");
                    }
                    return;
                }
            }

            s_extensionNamespaces.Add((namespacePrefix, namespaceUri));
            s_cachedSerializer.Clear();
        }

        public static void RegisterExtensionType(Type extensionType)
        {
            s_extensionTypes ??= new List<Type>();
            if (s_extensionTypes.Contains(extensionType)) { return; }

            s_extensionTypes.Add(extensionType);
            s_cachedSerializer.Clear();
        }

        public static XmlSerializer GetSerializer(GpxVersion version)
        {
            if (s_cachedSerializer.TryGetValue(version, out var cachedSerializer))
            {
                return cachedSerializer;
            }

            var defaultNamespace = version switch
            {
                GpxVersion.V1_1 => "http://www.topografix.com/GPX/1/1",
                GpxVersion.V1_0 => "http://www.topografix.com/GPX/1/0",
                _ => throw new ArgumentException($"Unknown gpx version {version}!", nameof(version))
            };

            XmlSerializer result;
            if (s_extensionTypes != null)
            {
                result = new XmlSerializer(typeof(GpxFile), null, s_extensionTypes.ToArray(), null, defaultNamespace);
            }
            else
            {
                result = new XmlSerializer(typeof(GpxFile), defaultNamespace);
            }

            s_cachedSerializer[version] = result;
            return result;
        }

        public static void Serialize(GpxFile gpxFile, TextWriter textWriter)
        {
            gpxFile.EnsureNamespaceDeclarations();

            // Copy given GpxFile object to new one to enable some modifications before serializing
            // The original GpxFile object will not be modified during this process
            var fileToSave = new GpxFile();
            fileToSave.Waypoints.AddRange(gpxFile.Waypoints);
            fileToSave.Extensions = gpxFile.Extensions;
            fileToSave.Routes.AddRange(gpxFile.Routes);
            fileToSave.Metadata = gpxFile.Metadata;
            fileToSave.Tracks.AddRange(gpxFile.Tracks);
            fileToSave.Creator = "RK GpxViewer";

            // Force http://www.topografix.com/GPX/1/1 to be default namespace
            if (gpxFile.Xmlns != null)
            {
                var namespaceArray = gpxFile.Xmlns.ToArray();
                var newNamespaces = new List<XmlQualifiedName>(namespaceArray.Length);
                for (var loop = 0; loop < namespaceArray.Length; loop++)
                {
                    if (namespaceArray[loop].Namespace == "http://www.topografix.com/GPX/1/0"){ continue; }
                    if (namespaceArray[loop].Namespace == "http://www.topografix.com/GPX/1/1"){ continue; }
                    if (string.IsNullOrEmpty(namespaceArray[loop].Name))
                    {
                        throw new InvalidOperationException(
                            $"Unknown default namespace {namespaceArray[loop].Namespace}");
                    }
                    newNamespaces.Add(namespaceArray[loop]);
                }
                newNamespaces.Insert(0, new XmlQualifiedName("", "http://www.topografix.com/GPX/1/1"));
                fileToSave.Xmlns = new XmlSerializerNamespaces(newNamespaces.ToArray());
            }
            else
            {
                fileToSave.Xmlns = new XmlSerializerNamespaces(new[]
                {
                    new XmlQualifiedName("", "http://www.topografix.com/GPX/1/1")
                });
            }
            fileToSave.Version = "1.1";

            // Serialization logic
            GetSerializer(GpxVersion.V1_1).Serialize(textWriter, fileToSave);
        }

        public static void Serialize(GpxFile gpxFile, Stream stream)
        {
            using(var streamWriter = new StreamWriter(stream))
            {
                Serialize(gpxFile, streamWriter);
            }
        }

        public static void Serialize(GpxFile gpxFile, string targetFile)
        {
            using(var streamWriter = new StreamWriter(File.Create(targetFile)))
            {
                Serialize(gpxFile, streamWriter);
            }
        }

        public static GpxFile Deserialize(TextReader textReader, GpxFileDeserializationMethod method)
        {
            switch (method)
            {
                case GpxFileDeserializationMethod.Compatibility:
                    // Read whole file to memory to do some checking / manipulations first
                    //  - Correct xml namespace
                    //  - Correct xml version (.Net does not support xml 1.1)
                    var fullText = textReader.ReadToEnd();
                    var gpxVersion = fullText.Contains("xmlns=\"http://www.topografix.com/GPX/1/1\"") ? GpxVersion.V1_1 : GpxVersion.V1_0;

                    using (var strReader = new StringReader(fullText))
                    {
                        // Discard initial xml header
                        // In this way the XmlSerializer does also try to read xml 1.1 content
                        if (fullText.StartsWith("<?xml"))
                        {
                            var endTagIndex = fullText.IndexOf("?>", StringComparison.Ordinal);
                            if (endTagIndex < 0)
                            {
                                throw new InvalidOperationException($"Unable to process xml declaration!");
                            }
                            strReader.ReadBlock(new char[endTagIndex + 2], 0, endTagIndex + 2);
                        }

                        // Try to deserialize
                        if (GetSerializer(gpxVersion).Deserialize(strReader) is not GpxFile result1)
                        {
                            throw new GpxFileException($"Unable to deserialize {nameof(GpxFile)}: Unknown error");
                        }
                        return result1;
                    }

                case GpxFileDeserializationMethod.OnlyGpx1_1:
                    if (GetSerializer(GpxVersion.V1_1).Deserialize(textReader) is not GpxFile result2)
                    {
                        throw new GpxFileException($"Unable to deserialize {nameof(GpxFile)}: Unknown error");
                    }
                    return result2;

                default:
                    throw new ArgumentException($"Unknown deserialization method {method}", nameof(method));
            }

        }

        public static GpxFile Deserialize(Stream stream, GpxFileDeserializationMethod method)
        {
            using var streamReader = new StreamReader(stream);

            return Deserialize(streamReader, method);

        }

        public static GpxFile Deserialize(string sourceFile, GpxFileDeserializationMethod method)
        {
            using var fileStream = File.OpenRead(sourceFile);

            return Deserialize(fileStream, method);
        }
    }
}
