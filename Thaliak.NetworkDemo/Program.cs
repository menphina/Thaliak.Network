using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Milvaneth.Common;
using Thaliak.Network;
using Thaliak.Network.Filter;
using Thaliak.Network.Messages;
using Thaliak.Network.Sniffer;
using Thaliak.Network.Utilities;

namespace Thaliak.NetworkDemo
{
    class Program
    {
        private static readonly List<string> LogLines = new List<string>();
        private static bool _isStopping;
        private static List<Connection> _conn = new List<Connection>();

        static void Main(string[] args)
        {
            // Find game process to start monitoring
            var p = Helper.GetProcess();
            // You should provide the namespace to search for message decoders.
            // The search will be taken place in AppDomain.CurrentDomain.
            var gnm = new GameNetworkMonitor(p, "Thaliak.Network.Messages");

            // Subscribe to messages by messageId, note that you should have a decoder on the message or you will receive NOTHING.
            gnm.Subscribe(NetworkMarketListing.GetMessageId(), DispWtsInfo);
            gnm.Subscribe(NetworkLogout.GetMessageId(), Logout);
            gnm.Subscribe(NetworkRetainerSummary.GetMessageId(), RetainerSummary);
            gnm.Subscribe(NetworkPlayerSpawn.GetMessageId(), WorldId);

            // Thaliak.Network will take snapshot to game connection and listen on it.
            // It will NOT automatically update connection list by design, so a state machine may be helpful.
            // You can call gnm.Update() at any time to update filters and working networking interface.
            gnm.Start();

            // A simple connection updater.
            Task.Run(() =>
            {
                var lobby = ConnectionPicker.GetLobbyEndPoint(p);

                for (;;)
                {
                    var conn = ConnectionPicker.GetConnections(p);

                    if (!conn.Any())
                    {
                        gnm.Stop();
                        return;
                    }

                    if (_conn.Count != conn.Count)
                    {
                        var filters = FilterBuilder.BuildDefaultFilter(conn);

                        filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.Remote, lobby,
                            MessageAttribute.DirectionSend | MessageAttribute.CatalogLobby));
                        filters.PropertyFilters.Add(new PropertyFilter<IPPacket>(x => x.Local, lobby,
                            MessageAttribute.DirectionReceive | MessageAttribute.CatalogLobby));

                        gnm.Update(conn.First().LocalEndPoint.Address, filters);
                    }

                    _conn = conn;

                    Thread.Sleep(100);
                }
            });

            // Some display stuff
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            PrintHeader(p);

            while (!_isStopping)
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

            if (LogLines.Count > 10)
            {
                LogLines.RemoveRange(0, LogLines.Count - 10);
            }

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine((i < LogLines.Count ? LogLines[i] : "").PadRight(40));
            }
            Console.SetCursorPosition(0, Console.CursorTop - 16);
        }

        private static void PrintHeader(Process p)
        {
            BringConsoleToFront();
            Console.WriteLine("");
            Console.WriteLine("Thaliak.Network - Milvaneth Network Sniffing Framework");
            Console.WriteLine("Monitoring FFXIV DX11 process PID {0}", p.Id);
            Console.WriteLine("Press Ctrl+C to Exit");
            Console.WriteLine();
        }

        private static void DispWtsInfo(NetworkMessageHeader header, IResult msg)
        {
            foreach (var item in ((MarketListingResult)msg).ListingItems)
            {
                LogLines.Add($"{item.RetainerName} WTS for {item.UnitPrice * item.Quantity + item.TotalTax}Gil (VAT incl.)".PadRight(40));
            }
        }

        private static void Logout(NetworkMessageHeader header, IResult msg)
        {
            LogLines.Add($"Logout");
        }

        private static void RetainerSummary(NetworkMessageHeader header, IResult msg)
        {
            LogLines.Add($"Retainer {((NetworkRetainerSummary)msg).RetainerName}");
        }

        private static void WorldId(NetworkMessageHeader header, IResult msg)
        {
            LogLines.Add($"You are now in World {((NetworkPlayerSpawn)msg).HomeWorldId}");
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _isStopping = true;
        }

        private static void BringConsoleToFront()
        {
            SetWindowPos(GetConsoleWindow(), new IntPtr(-1), 0, 0, 0, 0, 19);
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);
    }
}
