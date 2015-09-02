using System.Diagnostics;
using System.Text;
using System.Threading;
using CommonUtils.Core.Logger;

namespace AutoPublication.Code {
    public static class CmdCommandExecutor {
        /// <summary>
        /// Логгер.
        /// </summary>
        private static readonly LoggerWrapper _logger = LoggerManager.GetLogger(typeof(CmdCommandExecutor).FullName);
        
        public static string Run(string fileName, string command) {
            var pi = new ProcessStartInfo(fileName, command) {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                LoadUserProfile = true
            };
            var process = Process.Start(pi);
            var clock = new Stopwatch();
            clock.Start();
            do {
                if(clock.Elapsed.TotalSeconds >= 60 * 1000) {
                    process.Kill();
                }
                Thread.Sleep(10);
            } while(!process.HasExited);
            var standardOutputBuilder = new StringBuilder();
            while(!process.StandardOutput.EndOfStream) {
                standardOutputBuilder.AppendLine(process.StandardOutput.ReadLine());
            }
            while(!process.StandardError.EndOfStream) {
                standardOutputBuilder.AppendLine(process.StandardError.ReadLine());
            }
            var result = standardOutputBuilder.ToString();
            _logger.Info(string.Format("{0} {1} \r\n {2}", fileName, command, result));
            return result;
        }
    }
}