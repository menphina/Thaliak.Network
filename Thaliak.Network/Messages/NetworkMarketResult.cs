using System.Collections.Generic;
using System.Runtime.InteropServices;
using Thaliak.Network.Utilities;

namespace Thaliak.Network.Messages
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
            return MessageIdRetriver.Instance.GetMessageId(MessageIdRetriveKey.NetworkMarketResult);
        }

        public new static unsafe NetworkMarketResult Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkMarketResultRaw*) raw).Spawn(data, offset);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketResultRaw : INetworkMessageBase<NetworkMarketResult>
    {
        [FieldOffset(160)]
        public int ItemIndexEnd;

        [FieldOffset(164)]
        public int Padding;

        [FieldOffset(168)]
        public int ItemIndexStart;

        [FieldOffset(172)]
        public int RequestId;

        public NetworkMarketResult Spawn(byte[] data, int offset)
        {
            const int itemSize = 8;
            const int itemCount = 20;
            var items = new List<NetworkMarketResultItem>(itemCount);
            for (var i = 0; i < itemCount; i++)
            {
                fixed (byte* p = &data[offset + 0 + i * itemSize])
                {
                    var item = *(NetworkMarketResultItem*) p;
                    if(item.ItemId != 0)
                        items.Add(item);
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
