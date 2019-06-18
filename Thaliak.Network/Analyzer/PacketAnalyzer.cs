using Milvaneth.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thaliak.Network.Filter;
using Thaliak.Network.Sniffer;

namespace Thaliak.Network.Analyzer
{
    public sealed class PacketAnalyzer : ISnifferOutput
    {
        private readonly Filters<IPPacket> _markers;
        private readonly MemoryStream _buffer;
        private readonly IAnalyzerOutput _output;

        private BlockingCollection<AnalyzedPacket> _outputQueue;
        private bool _isStopping;
        private long _packetObserved;
        private long _messageProcessed;

        private const TCPFlags FlagAckPsh = TCPFlags.ACK | TCPFlags.PSH;
        private const TCPFlags FlagFinRst = TCPFlags.FIN | TCPFlags.RST;
        private const int HeaderLength = 40;

        public long PacketObserved => _packetObserved;
        public long MessageProcessed => _messageProcessed;

        public PacketAnalyzer(Filters<IPPacket> markers, IAnalyzerOutput output)
        {
            this._markers = markers;
            this._output = output;

            this._isStopping = false;
            this._buffer = new MemoryStream(65536);
        }

        public void Start()
        {
            this._outputQueue = new BlockingCollection<AnalyzedPacket>();

            Task.Factory.StartNew(() =>
            {
                foreach (var analyzedPacket in this._outputQueue.GetConsumingEnumerable())
                {
                    this._output.Output(analyzedPacket);
                }
            });
        }

        public void Stop()
        {
            this._isStopping = true;
        }

        public unsafe void Output(TimestampedData timestampedData)
        {
            var ipPacket = timestampedData.Packet;

            if (ipPacket?.Protocol != 6) return; // TCP

            var tcpPacket = new TCPPacket(timestampedData.Data, ipPacket.HeaderLength);

            if (!tcpPacket.IsValid || (tcpPacket.Flags & FlagAckPsh) != FlagAckPsh)
            {
                if ((tcpPacket.Flags & FlagFinRst) != 0)
                {
                    Notifier.Raise(Signal.ClientDisconnected, null);
                }

                return;
            }

            if(tcpPacket.Payload == null) return;

            Interlocked.Increment(ref this._packetObserved);

            var mark = _markers.WhichMatch(ipPacket);

            _buffer.Seek(0, SeekOrigin.End); // pos = end
            _buffer.Write(tcpPacket.Payload, 0, tcpPacket.Payload.Length); // pos = end

            var processedLength = 0;
            var needPurge = false;
            var currentHeader = new byte[HeaderLength];

            for (;;)
            {
                if (!needPurge)
                {
                    _buffer.Seek(processedLength, SeekOrigin.Begin); // pos = 0

                    if (_buffer.Length - processedLength <= HeaderLength || processedLength > 65536) // not enough data
                    {
                        var remaining = _buffer.Length - processedLength;
                        var tmp = new byte[remaining];
                        _buffer.Read(tmp, 0, tmp.Length);
                        _buffer.SetLength(0);
                        _buffer.Write(tmp, 0, tmp.Length);
                        return;
                    }

                    _buffer.Read(currentHeader, 0, HeaderLength); // pos = 40


                    if (!IsValidHeader(currentHeader))
                    {
                        needPurge = true;
                        continue;
                    }

                    NetworkPacketHeader header;
                    fixed (byte* p = &currentHeader[0])
                    {
                        header = *(NetworkPacketHeader*) p;
                    }

                    // TODO: cast

                    var timeDelta = Math.Abs(Helper.DateTimeToUnixTimeStamp(DateTime.Now) * 1000 - header.Timestamp);

                    if (header.Length < HeaderLength || (header.Timestamp != 0 && timeDelta > 60000)) // > 1 min
                    {
                        needPurge = true;
                        continue;
                    }

                    if (header.Length > _buffer.Length - processedLength) return; // wait for more content

                    var content = GenerateContent(_buffer, header.IsCompressed, header.Length);

                    if (content.Length == 0)
                    {
                        content.Dispose();

                        needPurge = true;
                        continue;
                    }

                    ConsumeContent(content, header, mark);

                    processedLength += header.Length; // pos = 0
                }
                else
                {
                    needPurge = false;

                    var bytes = _buffer.ToArray();
                    var pos = FindMagic(new ArraySegment<byte>(bytes, 1, bytes.Length - 1));

                    if (pos == -1)
                    {
                        _buffer.SetLength(0); // no available data, drop all content
                        return;
                    }

                    processedLength += pos;
                }
            }
        }

        private MemoryStream GenerateContent(Stream stream, bool isCompressed, ushort length, int offset = -1)
        {
            if (offset >= 0)
            {
                stream.Seek(offset, SeekOrigin.Begin);
            }

            var content = new MemoryStream();
            if (isCompressed)
            {
                var body = new byte[length - HeaderLength - 2];
                stream.Seek(2, SeekOrigin.Current);
                stream.Read(body, 0, body.Length); // pos = length

                try
                {
                    using (var input = new MemoryStream(body))
                    using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
                    {
                        deflate.CopyTo(content);
                    }
                }
                catch
                {
                    Notifier.Raise(Signal.ClientPacketParseFail, new[] {"unable to deflate"});
                }
            }
            else
            {
                var body = new byte[length - HeaderLength];
                stream.Read(body, 0, body.Length); // pos = bundle.Length
                content = new MemoryStream(body);
            }

            return content;
        }

        private void ConsumeContent(Stream content, NetworkPacketHeader header, List<MessageAttribute> mark)
        {
            var actualLen = 0;
            content.Seek(0, SeekOrigin.Begin);

            while (header.Count-- > 0)
            {
                var lenBytes = new byte[4];
                content.Read(lenBytes, 0, 4);
                var len = BitConverter.ToInt32(lenBytes, 0);

                actualLen += len;
                if (actualLen > content.Length) break;

                var msg = new byte[len];
                content.Read(msg, 4, len - 4);

                Buffer.BlockCopy(lenBytes, 0, msg, 0, 4);

                EnqueueOutput(new AnalyzedPacket(len, msg, header.Timestamp, mark));
                Interlocked.Increment(ref this._messageProcessed);
            }

            content.Dispose();
        }

        private void EnqueueOutput(AnalyzedPacket analyzedPacket)
        {
            if (this._isStopping)
            {
                this._outputQueue.CompleteAdding();
                return;
            }

            this._outputQueue.Add(analyzedPacket);
        }

        private int FindMagic(IList<byte> target)
        {
            ulong headerRR, header00;

            unchecked
            {
                headerRR = (uint) Searcher.Search(target, new byte[] {0x52, 0x52, 0xa0, 0x41});
                header00 = (uint) Searcher.Search(target,
                    new byte[]
                    {
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                    });
            }

            return (int)Math.Min(headerRR, header00);
        }

        private bool IsValidHeader(byte[] target)
        {
            var magic = BitConverter.ToUInt32(target, 0);
            if (magic != 0x41a05252u && magic != 0x00000000u) return false;

            var magicBytes = target.Take(16).ToArray();
            if (magic == 0x00000000u && magicBytes.Any(x => x != 0)) return false;

            return true;
        }
    }
}
