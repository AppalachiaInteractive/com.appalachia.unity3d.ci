using System;
using UnityEngine;

namespace Appalachia.CI.Integration.Core.Shell
{
    internal static class ShellLogger
    {
        public static void Log<T>(string message)
        {
            var time = $"{Time.realtimeSinceStartup:F2}";

            var logPrefix = $"{time} {typeof(T).Name}";

            var log = $"{logPrefix} {message.Trim()}";

            Console.WriteLine(log);
        }

        public static void Log<T>(string processKey, string message)
        {
            var time = $"{Time.realtimeSinceStartup:F2}";

            var logPrefix = $"{time} {typeof(T).Name} '{processKey}>'";

            var log = $"{logPrefix} {message.Trim()}";

            Console.WriteLine(log);
        }

        public static void Log<T>(string processKey, double elapsed, string message)
        {
            var time = $"{Time.realtimeSinceStartup:F2}";

            var logPrefix = $"{time} {typeof(T).Name} '{processKey}' [{elapsed:F2} s]";

            var log = $"{logPrefix} {message.Trim()}";

            Console.WriteLine(log);
        }
    }
}
