using System;
using System.IO;

namespace MessageCommunicator.TestGui
{
    public class IntegratedDocFile
    {
        private Func<TextReader> _fileReader;
        private string? _fullContent;
        
        public string Title { get; }

        public IntegratedDocFile(string title, Func<TextReader> fileReader)
        {
            _fileReader = fileReader;
            
            this.Title = title;
        }

        public string ReadFullContent()
        {
            if (!string.IsNullOrEmpty(_fullContent)) { return _fullContent; }

            _fullContent = _fileReader().ReadToEnd();
            return _fullContent;
        }
    }
}