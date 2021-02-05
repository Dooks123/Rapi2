using System.Runtime.InteropServices;

namespace Rapi2.Interop
{
    [Guid("357a557c-b03f-4240-90d8-c6c71c659bf1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRAPIEnumDevices
    {
        IRAPIDevice Next();

        void Reset();

        void Skip(uint cElt);

        IRAPIEnumDevices Clone();

        int GetCount();
    }
}