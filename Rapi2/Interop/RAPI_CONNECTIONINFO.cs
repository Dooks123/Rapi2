using System.Net;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Sequential, Size = 264)]
    internal struct RAPI_CONNECTIONINFO
    {
        private readonly SOCKADDR_STORAGE _ipaddr;
        private readonly SOCKADDR_STORAGE _hostIpaddr;
        public ConnectionType connectionType;

        public IPEndPoint Ipaddr { get { return _ipaddr.IpAddress; } }
        public IPEndPoint HostIpaddr { get { return _hostIpaddr.IpAddress; } }
    }
}