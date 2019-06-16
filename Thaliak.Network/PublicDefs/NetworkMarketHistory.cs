using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Thaliak.Network
{
    public class NetworkMarketHistory : NetworkMessage
    {
        public int ItemId;
        public int ItemId1;
        public List<NetworkMarketHistoryItem> HistoryItems;

        public new static int GetMessageId()
        {
            return 0x012A;
        }

        public new static unsafe NetworkMarketHistory Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkMarketHistoryRaw*) raw).Spawn();
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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52 * 20)]
        [FieldOffset(8)]
        public fixed byte HistoryItem[52 * 20];

        public NetworkMarketHistory Spawn()
        {
            const int itemSize = 52;
            const int itemCount = 20;
            var items = new List<NetworkMarketHistoryItem>(itemCount);
            for (var i = 0; i < itemCount; i++)
            {
                fixed (byte* p = &HistoryItem[i * itemSize])
                {
                    items.Add((*(NetworkMarketHistoryItemRaw*)p).Spawn());
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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
        [FieldOffset(15)]
        public fixed byte BuyerName[33];

        [FieldOffset(48)]
        public int ItemId;

        public NetworkMarketHistoryItem Spawn()
        {
            fixed (byte* p = this.BuyerName)
            {
                return new NetworkMarketHistoryItem
                {
                    UnitPrice = this.UnitPrice,
                    PurchaseTime = this.PurchaseTime,
                    Quantity = this.Quantity,
                    IsHq = this.IsHq,
                    Padding = this.Padding,
                    OnMannequin = this.OnMannequin,
                    BuyerName = new string((sbyte*)p, 0, 33, Encoding.UTF8).Split('\0')[0],
                    ItemId = this.ItemId,
                };
            }
        }
    }
}
