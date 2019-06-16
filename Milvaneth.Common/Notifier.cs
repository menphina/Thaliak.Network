using System;

namespace Milvaneth.Common
{
    public class Notifier
    {
        public delegate void NotificationListener(Signal sig, DateTime time, string stack, string[] args);

        public static event NotificationListener OnNotification = GlobalIpcNotifier;

        public static void Raise(Signal sig, string[] args = null, bool noLog = false)
        {
            try
            {
                if(!noLog)
                    LogOutput.Instance.LogNotification(sig, DateTime.Now, Environment.StackTrace, args);

                OnNotification(sig, DateTime.Now, Environment.StackTrace, args);
            }
            catch
            {
                // Ignored
            }
        }

        private static void GlobalIpcNotifier(Signal sig, DateTime time, string stack, string[] args)
        {
            // TODO: named pipe or mutex, whatever
        }

        private static void GlobalIpcReceiver(Signal sig, DateTime time, string stack, string[] args)
        {
            // TODO: named pipe or mutex, whatever
            // Logs should be handled at sender side
            OnNotification(sig, DateTime.Now, Environment.StackTrace, args);
        }
    }
}
