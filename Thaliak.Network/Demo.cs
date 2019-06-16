using Milvaneth.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Thaliak.Network.Analyzer;
using Thaliak.Network.Dispatcher;
using Thaliak.Network.Sniffer;
using Thaliak.Network.Utilities;

namespace Thaliak.Network
{
    // Demo class is to show how to use Thaliak.Network
    // Call Demo.Run in a Console application to run the demo
    public class Demo
    {
        private static bool isStopping;
        private static List<string> visited = new List<string>();

        public static void Run()
        {
            var p = Helper.GetProcess();
            var nic = NetworkInterfaceInfo.GetDefaultInterface();
            var filters = FilterBuilder.BuildDefaultFilter(p);

            var dispatcher = new MessageDispatcher(new[]
            {
                typeof(NetworkMarketHistory),
                typeof(NetworkCharacterName),
                typeof(NetworkMarketListing),
                typeof(NetworkMarketListingCount),
                typeof(NetworkMarketResult),
                typeof(NetworkPlayerSpawn)
            });
            var analyzer = new PacketAnalyzer(filters, dispatcher);
            var sniffer = new SocketSniffer(nic, filters, analyzer);

            dispatcher.Subcribe(NetworkMarketListing.GetMessageId(), WhoAmIVisited);

            analyzer.Start();
            sniffer.Start();
            

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            PrintHeader(nic, p);

            while (!isStopping)
            {
                PrintStatus(sniffer, analyzer);

                Thread.Sleep(50);
            }

            sniffer.Stop();
            analyzer.Stop();
        }

        private static void PrintStatus(SocketSniffer sniffer, PacketAnalyzer analyzer)
        {
            BringConsoleToFront();
            Console.WriteLine("IP Packets Observed: {0}", sniffer.PacketsObserved);
            Console.WriteLine("IP Packets Captured: {0}", sniffer.PacketsCaptured);
            Console.WriteLine("Game Packets Observed: {0}", analyzer.PacketObserved);
            Console.WriteLine("Game Messages Processed: {0}", analyzer.MessageProcessed);
            Console.WriteLine("---");
            for (var i = 0; i < 15; i++)
            {
                Console.WriteLine(i < visited.Count ? visited[i] : "");
            }
            Console.SetCursorPosition(0, Console.CursorTop - 20);
        }

        private static void PrintHeader(NetworkInterfaceInfo nic, Process p)
        {
            Console.WriteLine("");
            Console.WriteLine("Thaliak.Network - Milvaneth Network Sniffing Framework");
            Console.WriteLine("Capturing on interface {0} ({1})", nic.Name, nic.IPAddress);
            Console.WriteLine("Monitoring FFXIV DX11 process PID {0}", p.Id);
            Console.WriteLine("Press Ctrl+C to Exit");
            Console.WriteLine();
        }

        private static void WhoAmIVisited(NetworkMessageHeader header, NetworkMessage msg)
        {
            visited.Clear();
            foreach (var item in ((NetworkMarketListing)msg).ListingItems)
            {
                visited.Add(item.ItemId != 0 ? $"{item.RetainerName} WTS {item.UnitPrice * item.Quantity + item.TotalTax} (VAT incl.)".PadRight(30) : "".PadRight(30));
            }
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            isStopping = true;
        }

        private static void BringConsoleToFront()
        {
            SetWindowPos(GetConsoleWindow(), new IntPtr(-1), 0,0,0,0,3);
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    }
}
