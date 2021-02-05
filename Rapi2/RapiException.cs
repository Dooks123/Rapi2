using System.ComponentModel;

namespace Rapi2
{
    /// <summary>
    /// An exception thrown by the RAPI2 set of interfaces.
    /// </summary>
    public class RapiException : Win32Exception
    {
        internal RapiException(int lastError) : base(lastError) { }
    }
}