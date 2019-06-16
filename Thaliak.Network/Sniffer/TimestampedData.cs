// Modifications copyright (C) 2019 Menphnia

using System;

namespace Thaliak.Network.Sniffer
{
    public class TimestampedData
    {
        public DateTime Timestamp { get; }
        public byte[] Data { get; }
        public IPPacket Packet { get; private set; }

        public TimestampedData(DateTime timestamp, byte[] data)
        {
            this.Timestamp = timestamp;
            this.Data = data;
        }

        public void SetPacket(IPPacket packet)
        {
            Packet = packet;
        }
    }
}