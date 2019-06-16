using Milvaneth.Common;
using System.Collections.Generic;

namespace Thaliak.Network.Analyzer
{
    public class AnalyzedPacket
    {
        public int Length { get; }
        public long Timestamp { get; }
        public List<MessageAttribute> RouteMark { get; }
        public byte[] Message { get; }

        public AnalyzedPacket(int len, byte[] msg, long time, List<MessageAttribute> mark)
        {
            Length = len;
            Message = msg;
            Timestamp = time;
            RouteMark = mark;
        }
    }
}
