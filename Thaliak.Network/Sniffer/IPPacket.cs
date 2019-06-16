// Modifications copyright (C) 2019 Menphnia

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Thaliak.Network.Utilities;

namespace Thaliak.Network.Sniffer
{
    // ReSharper disable InconsistentNaming
    public class IPPacket
    {
        public int Version { get; }
        public int HeaderLength { get; }
        public int Protocol { get; }
        public IPAddress SourceAddress { get; }
        public IPAddress DestAddress { get; }
        public ushort SourcePort { get; }
        public ushort DestPort { get; }
        public IPEndPoint Local { get; }
        public IPEndPoint Remote { get; }
        public Connection Connection { get; }
        public Dictionary<byte, byte[]> IPv6ExtHeaders { get; }

        public IPPacket(byte[] data)
        {
            var versionAndLength = data[0];
            this.Version = versionAndLength >> 4;

            if (this.Version == 6)
            {
                // TODO: Experimental
                var realHeader = FindRealHeader(data, out var len, out var dic);

                this.HeaderLength = len;

                this.Protocol = Convert.ToInt32(realHeader);
                this.SourceAddress = new IPAddress(data.Skip(8).Take(16).ToArray());
                this.DestAddress = new IPAddress(data.Skip(24).Take(16).ToArray());

                this.IPv6ExtHeaders = dic;
            }
            else if (this.Version == 4)
            {
                this.HeaderLength = (versionAndLength & 0x0F) << 2;

                this.Protocol = Convert.ToInt32(data[9]);
                this.SourceAddress = new IPAddress(BitConverter.ToUInt32(data, 12));
                this.DestAddress = new IPAddress(BitConverter.ToUInt32(data, 16));

                this.IPv6ExtHeaders = null;
            }
            else
            {
                return;
            }

            if (Enum.IsDefined(typeof(ProtocolsWithPort), this.Protocol))
            {
                unchecked
                {
                    this.SourcePort =
                        (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, this.HeaderLength));
                    this.DestPort =
                        (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, this.HeaderLength + 2));
                }
            }

            this.Local = new IPEndPoint(SourceAddress, SourcePort);
            this.Remote = new IPEndPoint(DestAddress, DestPort);

            this.Connection = new Connection(Local, Remote);
        }

        private byte FindRealHeader(byte[] data, out int offset, out Dictionary<byte, byte[]> headerData)
        {
            // From https://en.wikipedia.org/wiki/IPv6_packet
            // IPSec Encapsulating Security Payload (No. 50) and Reserved (No. 253, 254) are not handled
            // As they are de facto data but not skippable headers. (Protected or has no mean to parse)

            headerData = new Dictionary<byte, byte[]>();

            var nextHeader = data[6];
            offset = 40; // Fixed header length

            for(;;)
            {
                int headerLength;
                switch (nextHeader)
                {
                    case 0:
                    case 60:
                    case 43:
                    case 135:
                    case 139:
                    case 140:
                        headerLength = 8 + 8 * data[offset + 1];
                        break;
                    case 51:
                        headerLength = 8 + 4 * data[offset + 1];
                        if (headerLength % 8 == 4) headerLength += 4; // IPv6 is 8-octet aligned
                        break;
                    case 44:
                        headerLength = 8;
                        break;
                    default:
                        return nextHeader;
                }

                headerData[nextHeader] = data.Skip(offset).Take(headerLength).ToArray();

                nextHeader = data[offset];
                offset += headerLength;
            }
        }
    }

    /// <summary>
    /// Protocols that have a port abstraction
    /// </summary>
    internal enum ProtocolsWithPort
    {
        TCP = 6,
        UDP = 17,
        SCTP = 132
    }
}