using System;

namespace FirLib.Core.Checking
{
    public class FirLibCheckException : FirLibException
    {
        /// <summary>
        /// Creates a new CommonLibraryException object
        /// </summary>
        public FirLibCheckException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new CommonLibraryException object
        /// </summary>
        public FirLibCheckException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}