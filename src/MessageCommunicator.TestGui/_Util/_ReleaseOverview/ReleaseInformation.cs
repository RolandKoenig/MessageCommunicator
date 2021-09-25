using System;

namespace MessageCommunicator.TestGui
{
    public class ReleaseInformation
    {
        public Version Version { get; }
        
        internal ReleaseInformation(Version version)
        {
            this.Version = version;
        }
    }
}