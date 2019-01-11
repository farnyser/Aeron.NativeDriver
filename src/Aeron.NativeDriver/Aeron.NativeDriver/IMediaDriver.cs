using System;

namespace Aeron.NativeDriver
{
    public interface IMediaDriver : IDisposable
    {
        string DirectoryName { get; }

        ThreadingMode ThreadingMode { get; }

        bool DeleteDirectoryOnLaunch { get; }

        void Launch(ThreadingMode threadingMode = ThreadingMode.Dedicated, bool deleteDirectoryOnLaunch = false);

        void LaunchEmbedded(ThreadingMode threadingMode = ThreadingMode.Dedicated);
    }
}