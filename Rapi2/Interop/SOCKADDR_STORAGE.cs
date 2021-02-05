using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [StructLayout(LayoutKind.Explicit, Size = 128, CharSet = CharSet.Ansi)]
    internal struct SOCKADDR_STORAGE
    {
        [FieldOffset(0)]
        readonly short ss_family;
        [FieldOffset(2)]
        readonly ushort port;
        [FieldOffset(4)]
        readonly uint addr;

        public AddressFamily AddressFamily { get { return (AddressFamily)ss_family; } }
        public IPEndPoint IpAddress
        {
            get
            {
                try { return new IPEndPoint(addr, port); }
                catch (Exception ex) { Debug.WriteLine(string.Format("Invalid IP Address from {1:x}:{2}\n{0}", ex, addr, port)); }
                return new IPEndPoint(IPAddress.Any, 0);
            }
        }
    }
}