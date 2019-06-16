using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Milvaneth.Common
{
    public class Helper
    {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static long DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - dtDateTime).TotalSeconds;
        }

        public static IList<Process> GetProcessList(bool includeDx9 = false)
        {
            var dx9 = includeDx9
                ? Process.GetProcessesByName("ffxiv").Where(x =>
                    !x.HasExited && x.MainModule != null && x.MainModule.ModuleName == "ffxiv.exe").ToList()
                : new List<Process>();
            return Process.GetProcessesByName("ffxiv_dx11")
                .Where(x => !x.HasExited && x.MainModule != null && x.MainModule.ModuleName == "ffxiv_dx11.exe")
                .Union(dx9).ToList();
        }

        public static Process GetProcess(int pid = 0, bool includeDx9 = false)
        {
            var ffxivProcessList = GetProcessList(includeDx9);
            return pid != 0 ? ffxivProcessList.FirstOrDefault(x => x.Id == pid) : ffxivProcessList.OrderBy(x => x.StartTime).FirstOrDefault(); // Attach to the 'longest lived' session
        }

        public static string GetMilFilePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Milvaneth", filename);
        }
    }
}
