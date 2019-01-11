using System;
using System.Runtime.InteropServices;

namespace Aeron.NativeDriver
{
    internal static class NativeMediaDriver
    {
        [DllImport("aeron_driver", EntryPoint = "aeron_driver_context_init")]
        internal static extern int InitContext(out IntPtr context);

        [DllImport("aeron_driver", EntryPoint = "aeron_driver_init")]
        internal static extern int InitDriver(out IntPtr driver, IntPtr context);

        [DllImport("aeron_driver", EntryPoint = "aeron_driver_start")]
        internal static extern int StartDriver(IntPtr driver, bool startConductor);

        [DllImport("aeron_driver", EntryPoint = "aeron_driver_close")]
        internal static extern int CloseDriver(IntPtr driver);

        [DllImport("aeron_driver", EntryPoint = "aeron_driver_context_close")]
        internal static extern int CloseContext(IntPtr context);
    }
}
