using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.IO.Packaging;
using Newtonsoft.Json;

namespace MessageCommunicator.TestGui
{
    public class DataPackageFile : IDisposable
    {
        private const string FILE_DESCRIPTOR = "/_descriptor.json";
        private const string FILE_SINGLE_DATA_FILE = "/singleDataFile.json";

        private Package _package;
        private bool _isClosed;

        public DataPackageFile(string fileName, FileMode fileMode, FileAccess fileAccess)
        {
            _package = Package.Open(fileName, fileMode, fileAccess);
            _isClosed = false;
        }

        /// <summary>
        /// Reads the descriptor of this package.
        /// </summary>
        public DataPackageDescriptor ReadDescriptor()
        {
            if(_isClosed){ throw new ObjectDisposedException(nameof(DataPackageFile)); }

            var descriptorUri = new Uri(FILE_DESCRIPTOR, UriKind.Relative);
            if (_package.PartExists(descriptorUri))
            {
                var descriptorPart = _package.GetPart(descriptorUri);

                DataPackageDescriptor? result = null;
                using (var inStream = descriptorPart.GetStream(FileMode.Open, FileAccess.Read))
                {
                    result = SerializationHelper.DeserializeFromStream<DataPackageDescriptor>(inStream);
                }
                if(result == null){ result = new DataPackageDescriptor(); }

                return result;
            }
            else
            {
                return new DataPackageDescriptor();    
            }
        }

        public void WriteSingleFile<T>(T fileContents, string dataType)
            where T : class
        {
            if(_isClosed){ throw new ObjectDisposedException(nameof(DataPackageFile)); }

            var descriptor = new DataPackageDescriptor();
            descriptor.DataType = dataType;

            // Write descriptor
            var descriptorUri = new Uri(FILE_DESCRIPTOR, UriKind.Relative);
            if(_package.PartExists(descriptorUri)){ _package.DeletePart(descriptorUri); }

            var outPartDescriptor = _package.CreatePart(descriptorUri, "text/json");
            using (var outStream = outPartDescriptor.GetStream(FileMode.Create))
            {
                SerializationHelper.SerializeToStream(outStream, descriptor);
            }

            // Write content
            var contentUri = new Uri(FILE_SINGLE_DATA_FILE, UriKind.Relative);
            if(_package.PartExists(contentUri)){ _package.DeletePart(contentUri); }

            var outPartContent = _package.CreatePart(contentUri, "text/json");
            using (var outStream = outPartContent.GetStream(FileMode.Create))
            {
                SerializationHelper.SerializeToStream(outStream, fileContents);
            }
        }

        public T ReadSingleFile<T>(string dataType)
            where T : class
        {
            if (_isClosed) { throw new ObjectDisposedException(nameof(DataPackageFile)); }

            // Check descriptor
            var descriptorUri = new Uri(FILE_DESCRIPTOR, UriKind.Relative);
            if (!_package.PartExists(descriptorUri))
            {
                throw new InvalidOperationException("No descriptor found in package file!");
            }

            var descriptorPart = _package.GetPart(descriptorUri);
            using (var inStream = descriptorPart.GetStream(FileMode.Open, FileAccess.Read))
            {
                var descriptor = SerializationHelper.DeserializeFromStream<DataPackageDescriptor>(inStream);
                if (descriptor == null)
                {
                    throw new InvalidOperationException("Unable to deserialize descriptor from package file!");
                }
                else if (descriptor.DataType != dataType)
                {
                    throw new InvalidOperationException($"Invalid data type {descriptor.DataType}! (expected {dataType})");
                }
            }

            // Check and load data file
            var dataFileUri = new Uri(FILE_SINGLE_DATA_FILE, UriKind.Relative);
            if (!_package.PartExists(dataFileUri))
            {
                throw new InvalidOperationException("No data file found in package file!");
            }

            var dataFilePart = _package.GetPart(dataFileUri);
            using (var inStream = dataFilePart.GetStream(FileMode.Open, FileAccess.Read))
            {
                var result = SerializationHelper.DeserializeFromStream<T>(inStream);
                if (result == null)
                {
                    throw new InvalidOperationException($"Unable to deserialize data file!");
                }
                return result;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_isClosed)
            {
                _package.Close();
                _isClosed = true;
            }
        }
    }
}
