using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    public class IntegratedDocUtil
    {
        public static IntegratedDocUtil Current { get; } = new IntegratedDocUtil();

        public IEnumerable<Assembly> QueryAssemblies => _queryAssemblies;

        private List<Assembly> _queryAssemblies;
        private Dictionary<string, string> _loadedDocs;

        public IntegratedDocUtil()
        {
            _loadedDocs = new Dictionary<string, string>(12);

            _queryAssemblies = new List<Assembly>();
            _queryAssemblies.Add(Assembly.GetExecutingAssembly());
        }

        public void AddQueryAssembly(Assembly assembly)
        {
            if (_queryAssemblies.Contains(assembly))
            {
                throw new InvalidOperationException($"Assembly '{assembly}' already added!");
            }

            _queryAssemblies.Add(assembly);
        }

        public string GetIntegratedDoc(string docFileName)
        {
            if (string.IsNullOrEmpty(docFileName))
            {
                throw new ArgumentException("Given file name is null or empty!", nameof(docFileName));
            }

            if (_loadedDocs.TryGetValue(docFileName, out var preloadedDoc))
            {
                return preloadedDoc;
            }

            foreach (var currentAssembly in _queryAssemblies)
            {
                var nameWithDot = $".{docFileName}";
                foreach (var actResource in currentAssembly.GetManifestResourceNames())
                {
                    if (!actResource.EndsWith(nameWithDot)){ continue; }

                    var resStream = currentAssembly.GetManifestResourceStream(actResource);
                    if(resStream == null){ continue; }

                    using (var streamReader = new StreamReader(resStream))
                    {
                        var loadedDoc = streamReader.ReadToEnd();
                        _loadedDocs.Add(docFileName, loadedDoc);
                        return loadedDoc;
                    }
                }
            }

            throw new FileNotFoundException(
                $"Documentation file '{docFileName}' not found!",
                docFileName);
        }
    }
}
