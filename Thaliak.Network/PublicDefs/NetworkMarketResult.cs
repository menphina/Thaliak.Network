using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Thaliak.Network
{
    public class NetworkMarketResult : NetworkMessage
    {
        public List<NetworkMarketResultItem> ResultItems;
        public int ItemIndexEnd;
        public int Padding;
        public int ItemIndexStart;
        public int RequestId;

        public new static int GetMessageId()
        {
            return 0x0139;
        }

        public new static unsafe NetworkMarketResult Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkMarketResultRaw*) raw).Spawn();
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketResultRaw : INetworkMessageBase<NetworkMarketResult>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * 20)]
        [FieldOffset(0)]
        public fixed byte ResultItem[8 * 20];

        [FieldOffset(160)]
        public int ItemIndexEnd;

        [FieldOffset(164)]
        public int Padding;

        [FieldOffset(168)]
        public int ItemIndexStart;

        [FieldOffset(172)]
        public int RequestId;

        public NetworkMarketResult Spawn()
        {
            const int itemSize = 8;
            const int itemCount = 20;
            var items = new List<NetworkMarketResultItem>(itemCount);
            for (var i = 0; i < itemCount; i++)
            {
                fixed (byte* p = &ResultItem[i * itemSize])
                {
                    items.Add(*(NetworkMarketResultItem*)p);
                }
            }

            return new NetworkMarketResult
            {
                ResultItems = items,
                ItemIndexEnd = this.ItemIndexEnd,
                Padding = this.Padding,
                ItemIndexStart = this.ItemIndexStart,
                RequestId = this.RequestId,
            };
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct NetworkMarketResultItem
    {
        [FieldOffset(0)]
        public int ItemId;

        [FieldOffset(4)]
        public short OpenListing;

        [FieldOffset(6)]
        public short Demand;
    }
}
