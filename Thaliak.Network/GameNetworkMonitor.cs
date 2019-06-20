using System;
using System.Collections.Generic;
using System.Diagnostics;
using Thaliak.Network.Analyzer;
using Thaliak.Network.Dispatcher;
using Thaliak.Network.Filter;
using Thaliak.Network.Sniffer;
using Thaliak.Network.Utilities;

namespace Thaliak.Network
{
    // This is the 'default' entry class of Thaliak.Network, which bundles three sub-components
    public class GameNetworkMonitor
    {
        public Process ProcessWorking { get; }
        public NetworkInterfaceInfo NetworkInterface { get; }
        public Filters<IPPacket> FilterSet { get; }
        public IEnumerable<Type> ConsumerSet { get; }
        public long PacketsObserved => sniffer.PacketsObserved;
        public long PacketsCaptured => sniffer.PacketsCaptured;
        public long PacketsAnalyzed => analyzer.PacketsAnalyzed;
        public long MessagesProcessed => analyzer.MessagesProcessed;
        public long MessagesDispatched => dispatcher.MessagesDispatched;
        public long PacketsInQueue => sniffer.PacketsInQueue;
        public long MessagesInQueue => analyzer.MessagesInQueue;
        public long RequestsInQueue => dispatcher.RequestsInQueue;


        private readonly MessageDispatcher dispatcher;
        private readonly PacketAnalyzer analyzer;
        private readonly SocketSniffer sniffer;

        public GameNetworkMonitor(Process p)
        {
            this.ProcessWorking = p;

            var nic = NetworkInterfaceInfo.GetDefaultInterface();
            var filters = FilterBuilder.BuildDefaultFilter(p);
            var consumers = ConsumerSearcher.FindConsumers("Thaliak.Network.Messages");

            NetworkInterface = nic;
            FilterSet = filters;
            ConsumerSet = consumers;

            dispatcher = new MessageDispatcher(ConsumerSet);
            analyzer = new PacketAnalyzer(FilterSet, dispatcher);
            sniffer = new SocketSniffer(NetworkInterface, FilterSet, analyzer);
        }

        public GameNetworkMonitor(Process p, NetworkInterfaceInfo nic, Filters<IPPacket> filters, IEnumerable<Type> consumers)
        {
            this.ProcessWorking = p;

            NetworkInterface = nic;
            FilterSet = filters;
            ConsumerSet = consumers;

            dispatcher = new MessageDispatcher(ConsumerSet);
            analyzer = new PacketAnalyzer(FilterSet, dispatcher);
            sniffer = new SocketSniffer(NetworkInterface, FilterSet, analyzer);
        }

        public void Start()
        {
            dispatcher.Start();
            analyzer.Start();
            sniffer.Start();
        }
        
        public void Stop()
        {
            sniffer.Stop();
            analyzer.Stop();
            dispatcher.Stop();
        }

        public void Subcribe(int opcode, MessageDecoded listener)
        {
            dispatcher.Subcribe(opcode, listener);
        }

        public void Unsubcribe(int opcode, MessageDecoded listener)
        {
            dispatcher.Unsubcribe(opcode, listener);
        }
    }
}
