using Milvaneth.Common;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Thaliak.Network.Utilities;

namespace Thaliak.Network.Messages
{
    public class NetworkMarketHistory : NetworkMessage
    {
        public int ItemId;
        public int ItemId1;
        public List<NetworkMarketHistoryItem> HistoryItems;

        public new static int GetMessageId()
        {
            return MessageIdRetriver.Instance.GetMessageId(MessageIdRetriveKey.NetworkMarketHistory);
        }

        public new static unsafe NetworkMarketHistory Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkMarketHistoryRaw*) raw).Spawn(data, offset);
            }
        }
    }

    public class NetworkMarketHistoryItem
    {
        public int UnitPrice;
        public int PurchaseTime;
        public int Quantity;
        public byte IsHq;
        public byte Padding;
        public byte OnMannequin;
        public string BuyerName;
        public int ItemId;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketHistoryRaw : INetworkMessageBase<NetworkMarketHistory>
    {
        [FieldOffset(0)]
        public int ItemId;

        [FieldOffset(4)]
        public int ItemId1;

        public NetworkMarketHistory Spawn(byte[] data, int offset)
        {
            const int itemSize = 52;
            const int itemCount = 20;
            var items = new List<NetworkMarketHistoryItem>(itemCount);
            for (var i = 0; i < itemCount; i++)
            {
                fixed (byte* p = &data[offset + 8 + i * itemSize])
                {
                    var item = (*(NetworkMarketHistoryItemRaw*) p).Spawn(data, offset + 8 + i * itemSize);
                    if(item.ItemId != 0)
                        items.Add(item);
                }
            }

            return new NetworkMarketHistory
            {
                HistoryItems = items,
                ItemId = this.ItemId,
                ItemId1 = this.ItemId1,
            };
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketHistoryItemRaw : INetworkMessageBase<NetworkMarketHistoryItem>
    {
        [FieldOffset(0)]
        public int UnitPrice;

        [FieldOffset(4)]
        public int PurchaseTime;

        [FieldOffset(8)]
        public int Quantity;

        [FieldOffset(12)]
        public byte IsHq;

        [FieldOffset(13)]
        public byte Padding;

        [FieldOffset(14)]
        public byte OnMannequin;

        [FieldOffset(48)]
        public int ItemId;

        public NetworkMarketHistoryItem Spawn(byte[] data, int offset)
        {
            return new NetworkMarketHistoryItem
            {
                UnitPrice = this.UnitPrice,
                PurchaseTime = this.PurchaseTime,
                Quantity = this.Quantity,
                IsHq = this.IsHq,
                Padding = this.Padding,
                OnMannequin = this.OnMannequin,
                BuyerName = Helper.ToUtf8String(data, offset, 15, 33),
                ItemId = this.ItemId,
            };
        }
    }
}
