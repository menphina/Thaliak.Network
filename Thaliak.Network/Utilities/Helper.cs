using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Thaliak.Network
{
    public static class Helper
    {
        private const long TicksEpochTime = 621355968000000000L;
        private const long TicksPerSecond = 10000000;

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(TicksEpochTime + TicksPerSecond * unixTimeStamp).ToLocalTime();
        }

        public static long DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - TicksEpochTime) / TicksPerSecond;
        }

        public static IList<Process> GetProcessList(bool includeDx9 = false)
        {
            var dx9 = includeDx9
                ? Process.GetProcessesByName("ffxiv").Where(x =>
                    !x.HasExited && Path.GetFileName(x.GetMainModuleFileName()) == "ffxiv.exe").ToList()
                : new List<Process>();
            return Process.GetProcessesByName("ffxiv_dx11")
                .Where(x => !x.HasExited && Path.GetFileName(x.GetMainModuleFileName()) == "ffxiv_dx11.exe")
                .Union(dx9).ToList();
        }

        public static Process GetProcess(int pid = 0, bool includeDx9 = false)
        {
            var ffxivProcessList = GetProcessList(includeDx9);
            return pid != 0
                ? ffxivProcessList.FirstOrDefault(x => x.Id == pid)
                : ffxivProcessList.OrderBy(x => x.StartTime).FirstOrDefault(); // Attach to the 'longest lived' session
        }

        public static unsafe string ToUtf8String(byte[] arr, int off, int idx, int len)
        {
            fixed (byte* p = &arr[off])
            {
                return new string((sbyte*) p, idx, len, Encoding.UTF8).Split('\0')[0];
            }
        }

        // https://stackoverflow.com/questions/5497064/how-to-get-the-full-path-of-running-process/48319879#48319879
        public static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            var fileNameBuilder = new StringBuilder(buffer);
            var bufferLength = (uint) fileNameBuilder.Capacity + 1;
            return NativeMethods.QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength)
                ? fileNameBuilder.ToString()
                : null;
        }

        private static class NativeMethods
        {
            [DllImport("Kernel32.dll")]
            public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags,
                [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);
        }
    }
}
