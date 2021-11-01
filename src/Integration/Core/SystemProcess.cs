using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.Repositories;

namespace Appalachia.CI.Integration.Core
{
    public static class SystemShell
    {
        public static IEnumerator Execute(
            string command,
            RepositoryMetadata repository,
            Result result = null,
            DataReceivedEventHandler standardOutHandler = null,
            DataReceivedEventHandler standardErrorHandler = null,
            bool synchronous = false)
        {
            return Execute(
                command,
                repository.Path,
                result,
                standardOutHandler,
                standardErrorHandler,
                synchronous
            );
        }

        public static IEnumerator Execute(
            string command,
            string workingDir,
            Result result = null,
            DataReceivedEventHandler standardOutHandler = null,
            DataReceivedEventHandler standardErrorHandler = null,
            bool synchronous = false)
        {
            var outBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            var bash = @"C:\Program Files\Git\bin\bash.exe";

            var linuxWorkingDirectory = workingDir.ToAbsolutePath().WindowsToLinuxPath();

            Environment.SetEnvironmentVariable("APPA_FAST",        "1");
            Environment.SetEnvironmentVariable("APPA_PWD",         linuxWorkingDirectory);
            Environment.SetEnvironmentVariable("APPA_LOAD_BASHRC", "1");

            var processStartInfo = new ProcessStartInfo(bash, "-c \" " + command + " \"")
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var process = new Process {StartInfo = processStartInfo, EnableRaisingEvents = true};

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                {
                    return;
                }

                outBuilder.AppendLine(args.Data);
                standardOutHandler?.Invoke(sender, args);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                {
                    return;
                }

                errorBuilder.AppendLine(args.Data);
                standardErrorHandler?.Invoke(sender, args);
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (synchronous)
            {
                process.WaitForExit();
            }
            else
            {
                while (!process.HasExited)
                {
                    yield return null;
                }
            }

            if (result != null)
            {
                result.exitCode = process.ExitCode;
                result.error = errorBuilder.ToString();
                result.output = outBuilder.ToString();
            }

            process.Close();
        }

        #region Nested Types

        public class Result
        {
            public int exitCode;
            public string error;
            public string output;
        }

        #endregion
    }
}
