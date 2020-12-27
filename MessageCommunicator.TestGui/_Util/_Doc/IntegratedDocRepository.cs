using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MessageCommunicator.TestGui
{
    public class IntegratedDocRepository
    {
        private List<IntegratedDocFile> _lstAllFiles;
        private Dictionary<string, IntegratedDocFile> _dictAllFiles;

        public IntegratedDocRepository(Assembly source)
        {
            _lstAllFiles = new List<IntegratedDocFile>(16);
            _dictAllFiles = new Dictionary<string, IntegratedDocFile>(16);
            
            foreach (var actEmbeddedRes in source.GetManifestResourceNames())
            {
                if(!actEmbeddedRes.EndsWith(".md", StringComparison.OrdinalIgnoreCase)){ continue; }

                var actEmbeddedResInner = actEmbeddedRes;
                var docFile = ProcessMDFile(
                    () =>
                    {
                        var resourceStream = source.GetManifestResourceStream(actEmbeddedResInner);
                        if (resourceStream == null)
                        {
                            throw new ApplicationException(
                                $"Unable to open stream to file {actEmbeddedResInner} from assembly {source.FullName}!");
                        }
                        return new StreamReader(resourceStream);
                    });
                
                if (docFile != null)
                {
                    if (_dictAllFiles.ContainsKey(docFile.Title))
                    {
                        throw new ApplicationException($"Duplicate documentation file title {docFile.Title}!");
                    }
                    
                    _lstAllFiles.Add(docFile);
                    _dictAllFiles.Add(docFile.Title, docFile);
                }
            }
        }

        public IntegratedDocFile GetByTitle(string title)
        {
            if (_dictAllFiles.TryGetValue(title, out var foundFile))
            {
                return foundFile;
            }
            throw new FileNotFoundException($"Unable to find documentation file by title '{title}'", title);
        }

        public static IntegratedDocFile? ProcessMDFile(Func<TextReader> fileReader)
        {
            using (var stream = fileReader())
            {
                // Read title from first line
                var firstLine = stream.ReadLine();
                if (string.IsNullOrEmpty(firstLine)) { return null; } // <-- No content at all or first line is empty
                if (!firstLine.StartsWith("# ")){ return null; }      // <-- Invalid title
                if (firstLine.Length <= 2) { return null; }           // <-- Invalid title
                
                var title = firstLine.Substring(2).Trim();
                if (string.IsNullOrEmpty(title)) { return null; }     // <-- Invalid title
                
                return new IntegratedDocFile(
                    title,
                    fileReader);
            }
        }
    }
}