using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Aeron.NativeDriver
{
    public class MediaDriver : IMediaDriver
    {
        private static readonly string _aeronThreadingModeEnvVar = "AERON_THREADING_MODE";
        private static readonly string _aeronDirDeleteOnStartEnvVar = "AERON_DIR_DELETE_ON_START";
        private static readonly string _aeronDirEnvVar = "AERON_DIR";

        private static readonly Dictionary<ThreadingMode,string> _aeronThreadingModeLookup = new Dictionary<ThreadingMode,string>()
        {
            [ThreadingMode.Shared] = "SHARED",
            [ThreadingMode.SharedNetwork] = "SHARED_NETWORK",
            [ThreadingMode.Dedicated] = "DEDICATED",
        };

        private IntPtr _context;
        private IntPtr _driver;

        public string DirectoryName { get; private set; }
        public ThreadingMode ThreadingMode { get; private set; }
        public bool DeleteDirectoryOnLaunch { get; private set; }

        public void Launch(ThreadingMode threadingMode, bool deleteDirectoryOnLaunch)
        {
            DirectoryName = Path.Combine(Path.GetTempPath(), $"aeron-{Environment.UserName}");
            ThreadingMode = threadingMode;
            DeleteDirectoryOnLaunch = deleteDirectoryOnLaunch;

            LaunchInternal();
        }

        public void LaunchEmbedded(ThreadingMode threadingMode)
        {
            DirectoryName = Path.Combine(Path.GetTempPath(), $"aeron-embed-{Guid.NewGuid()}");
            ThreadingMode = threadingMode;
            DeleteDirectoryOnLaunch = true;

            LaunchInternal();
        }

        private void LaunchInternal()
        {
            Environment.SetEnvironmentVariable(_aeronDirDeleteOnStartEnvVar, DeleteDirectoryOnLaunch ? "true" : "false");
            Environment.SetEnvironmentVariable(_aeronDirEnvVar, DirectoryName);
            Environment.SetEnvironmentVariable(_aeronThreadingModeEnvVar, _aeronThreadingModeLookup[ThreadingMode]);

            if (NativeMediaDriver.InitContext(out _context) != 0)
                throw new Exception("Failed to init context");

            if (NativeMediaDriver.InitDriver(out _driver, _context) != 0)
                throw new Exception("Failed to init driver");

            if (NativeMediaDriver.StartDriver(_driver, false) != 0)
                throw new Exception("Failed to start driver");
        }

        public void Dispose()
        {
            NativeMediaDriver.CloseDriver(_driver);
            NativeMediaDriver.CloseContext(_context);
        }
    }
}
