using System;
using System.Threading;

namespace Aeron.NativeDriver.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting driver...");

            Console.CancelKeyPress += delegate { IsRunning = false; };

            using (var driver = new MediaDriver())
            {
                driver.LaunchEmbedded(ThreadingMode.Dedicated);

                while (IsRunning)
                {
                    Thread.Yield();
                }
            }
        }

        public static bool IsRunning { get; set; } = true;
    }
}
