using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Aeron.NativeDriver
{
    public class MediaDriver : IMediaDriver
    {
        private static readonly string _aeronThreadingModeEnvVar = "AERON_THREADING_MODE_ENV_VAR";
        private static readonly string _aeronDirDeleteOnStartEnvVar = "AERON_DIR_DELETE_ON_START_ENV_VAR";
        private static readonly string _aeronDirEnvVar = "AERON_DIR_ENV_VAR";

        private static readonly Dictionary<ThreadingMode,string> _aeronThreadingModeLookup = new Dictionary<ThreadingMode,string>()
        {
            [ThreadingMode.Shared] = "SHARED",
            [ThreadingMode.SharedNetwork] = "SHARED_NETWORK",
            [ThreadingMode.Dedicated] = "DEDICATED",
        };

        private readonly Process _process;

        public string DirectoryName { get; private set; }
        public ThreadingMode ThreadingMode { get; private set; }
        public bool DeleteDirectoryOnLaunch { get; private set; }

        public MediaDriver()
        {
            _process = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "aeronmd",
                }
            };

            _process.Exited += ProcessOnExited;
        }

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

        private void ProcessOnExited(object sender, EventArgs e)
        {
            throw new Exception($"Media driver exited unexpectedly");
        }

        private void LaunchInternal()
        {
            _process.StartInfo.EnvironmentVariables[_aeronDirEnvVar] = DirectoryName;
            _process.StartInfo.EnvironmentVariables[_aeronDirDeleteOnStartEnvVar] = DeleteDirectoryOnLaunch ? "true" : "false";
            _process.StartInfo.EnvironmentVariables[_aeronThreadingModeEnvVar] = _aeronThreadingModeLookup[ThreadingMode];
            _process.Start();
        }

        public void Dispose()
        {
            // TODO Send SIG_INT to native media driver
            _process.Exited -= ProcessOnExited;
            _process.Dispose();
        }
    }
}
