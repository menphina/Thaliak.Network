using Milvaneth.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Thaliak.Network.Messages;
using Thaliak.Network.Utilities;

namespace Thaliak.Network
{
    // Demo class to show how to use Thaliak.Network
    // Call Demo.Run in a Console application to run the demo
    public class Demo
    {
        private static bool isStopping;
        private static List<string> logLines = new List<string>();

        public static void Run()
        {
#pragma warning disable CS0618
            MessageIdRetriver.SetMessageId();
#pragma warning restore CS0618

            var p = Helper.GetProcess();
            var gnm = new GameNetworkMonitor(p);

            gnm.Subcribe(NetworkMarketListing.GetMessageId(), WhoAmIVisited);
            gnm.Subcribe(NetworkLogout.GetMessageId(), Logout); // recv -> waitfor25s -> no cancel -> logout
            gnm.Subcribe(NetworkRetainerSummary.GetMessageId(), RetainerRecv);

            gnm.Start();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            PrintHeader(gnm.NetworkInterface, p);

            while (!isStopping)
            {
                PrintStatus(gnm);

                Thread.Sleep(50);
            }

            gnm.Stop();
        }

        private static void PrintStatus(GameNetworkMonitor gnm)
        {
            Console.WriteLine("Packets Observed: {0}", gnm.PacketsObserved);
            Console.WriteLine("Packets Captured: {0}", gnm.PacketsCaptured);
            Console.WriteLine("Packets Analyzed: {0}", gnm.PacketsAnalyzed);
            Console.WriteLine("Messages Processed: {0}", gnm.MessagesProcessed);
            Console.WriteLine("Messages Dispatched: {0}", gnm.MessagesDispatched);
            Console.WriteLine("---");

            if (logLines.Count > 10)
            {
                logLines.RemoveRange(0, logLines.Count - 10);
            }

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(i < logLines.Count ? logLines[i] : "".PadRight(40));
            }
            Console.SetCursorPosition(0, Console.CursorTop - 16);
        }

        private static void PrintHeader(NetworkInterfaceInfo nic, Process p)
        {
            BringConsoleToFront();
            Console.WriteLine("");
            Console.WriteLine("Thaliak.Network - Milvaneth Network Sniffing Framework");
            Console.WriteLine("Capturing on interface {0} ({1})", nic.Name, nic.IPAddress);
            Console.WriteLine("Monitoring FFXIV DX11 process PID {0}", p.Id);
            Console.WriteLine("Press Ctrl+C to Exit");
            Console.WriteLine();
        }

        private static void WhoAmIVisited(NetworkMessageHeader header, NetworkMessage msg)
        {
            foreach (var item in ((NetworkMarketListing)msg).ListingItems)
            {
                logLines.Add($"{item.RetainerName} WTS for {item.UnitPrice * item.Quantity + item.TotalTax}Gil (VAT incl.)".PadRight(40));
            }
        }

        private static void Logout(NetworkMessageHeader header, NetworkMessage msg)
        {
            logLines.Add($"Logout".PadRight(40));
        }

        private static void RetainerRecv(NetworkMessageHeader header, NetworkMessage msg)
        {
            logLines.Add($"Retainer {((NetworkRetainerSummary)msg).RetainerName}");
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            isStopping = true;
        }

        private static void BringConsoleToFront()
        {
            SetWindowPos(GetConsoleWindow(), new IntPtr(-1), 0,0,0,0,19);
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    }
}
