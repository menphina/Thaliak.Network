using Serilog;
using System;
using System.Text;

namespace Milvaneth.Common
{
    public class LogOutput
    {
        public static string LogPath { get; } = Helper.GetMilFilePath("signal.log");
        public static LogOutput Instance => LazyInstance.Value;

        private static readonly Lazy<LogOutput> LazyInstance = new Lazy<LogOutput>(() => new LogOutput());

        private LogOutput()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(LogPath, rollingInterval:RollingInterval.Day)
                .CreateLogger();
        }

        public void LogNotification(Signal sig, DateTime time, string stack, string[] args)
        {
            try
            {
                var signal = Enum.GetName(typeof(Signal), sig);
                if (string.IsNullOrEmpty(signal))
                    signal = $"SIGINT{(int) sig}";

                var sb = new StringBuilder();
                var counter = 0;
                sb.AppendLine($"Signal: {signal}");
                sb.AppendLine("Stacktrace:");
                sb.AppendLine(stack);
                sb.AppendLine("Arguments:");
                foreach (var i in args)
                {
                    sb.AppendLine($"   [{counter++}] = {i}");
                }

                Log.Error(sb.ToString());
            }
            catch
            {
                // ignored
            }
        }
    }
}
