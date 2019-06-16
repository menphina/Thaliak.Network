using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Thaliak.Network
{
    public class NetworkMarketListing : NetworkMessage
    {
        public List<NetworkMarketListingItem> ListingItems;
        public byte ListingIndexEnd;
        public byte ListingIndexStart;
        public short RequestId;
        public short Padding;

        public new static int GetMessageId()
        {
            return 0x0126;
        }

        public new static unsafe NetworkMarketListing Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkMarketListingRaw*) raw).Spawn();
            }
        }
    }

    public class NetworkMarketListingItem
    {
        public long ListingId;
        public long RetainerId;
        public long OwnerId;
        public long CrafterId;
        public int UnitPrice;
        public int TotalTax;
        public int Quantity;
        public int ItemId;
        public int UpdateTime;
        public short Unknown1;
        public short OrderInList;
        public short Quality;
        public short Unknown2;
        public short Materia1;
        public short Materia2;
        public short Materia3;
        public short Materia4;
        public short Materia5;
        public short Unknown3;
        public int Unknown4;
        public string RetainerName;
        public string PlayerName;
        public byte IsHq;
        public byte MateriaCount;
        public byte OnMannequin;
        public byte RetainerLocation;
        public int Unknown5;
        public int Unknown6;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketListingRaw : INetworkMessageBase<NetworkMarketListing>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 152 * 10)]
        [FieldOffset(0)]
        public fixed byte ListingItem[152 * 10];

        [FieldOffset(1520)]
        public byte ListingIndexEnd;

        [FieldOffset(1521)]
        public byte ListingIndexStart;

        [FieldOffset(1522)]
        public short RequestId;

        [FieldOffset(1524)]
        public short Padding;

        public NetworkMarketListing Spawn()
        {
            const int itemSize = 152;
            const int itemCount = 10;
            var items = new List<NetworkMarketListingItem>(itemCount);
            for (var i = 0; i < itemCount; i++)
            {
                fixed (byte* p = &ListingItem[i * itemSize])
                {
                    items.Add((*(NetworkMarketListingItemRaw*)p).Spawn());
                }
            }

            return new NetworkMarketListing
            {
                ListingItems = items,
                ListingIndexEnd = this.ListingIndexEnd,
                ListingIndexStart = this.ListingIndexStart,
                RequestId = this.RequestId,
                Padding = this.Padding,
            };
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketListingItemRaw : INetworkMessageBase<NetworkMarketListingItem>
    {
        [FieldOffset(0)]
        public long ListingId;

        [FieldOffset(8)]
        public long RetainerId;

        [FieldOffset(16)]
        public long OwnerId;

        [FieldOffset(24)]
        public long CrafterId;

        [FieldOffset(32)]
        public int UnitPrice;

        [FieldOffset(36)]
        public int TotalTax;

        [FieldOffset(40)]
        public int Quantity;

        [FieldOffset(44)]
        public int ItemId;

        [FieldOffset(48)]
        public int UpdateTime;

        [FieldOffset(52)]
        public short Unknown1;

        [FieldOffset(54)]
        public short OrderInList;

        [FieldOffset(56)]
        public short Quality;

        [FieldOffset(58)]
        public short Unknown2;

        [FieldOffset(60)]
        public short Materia1;

        [FieldOffset(62)]
        public short Materia2;

        [FieldOffset(64)]
        public short Materia3;

        [FieldOffset(66)]
        public short Materia4;

        [FieldOffset(68)]
        public short Materia5;

        [FieldOffset(70)]
        public short Unknown3;

        [FieldOffset(72)]
        public int Unknown4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        [FieldOffset(76)]
        public fixed byte RetainerName[32];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        [FieldOffset(108)]
        public fixed byte PlayerName[32];

        [FieldOffset(140)]
        public byte IsHq;

        [FieldOffset(141)]
        public byte MateriaCount;

        [FieldOffset(142)]
        public byte OnMannequin;

        [FieldOffset(143)]
        public byte RetainerLocation;

        [FieldOffset(144)]
        public int Unknown5;

        [FieldOffset(148)]
        public int Unknown6;

        public NetworkMarketListingItem Spawn()
        {
            fixed (byte* p = this.RetainerName)
            fixed (byte* q = this.PlayerName)
            {
                return new NetworkMarketListingItem
                {
                    ListingId = this.ListingId,
                    RetainerId = this.RetainerId,
                    OwnerId = this.OwnerId,
                    CrafterId = this.CrafterId,
                    UnitPrice = this.UnitPrice,
                    TotalTax = this.TotalTax,
                    Quantity = this.Quantity,
                    ItemId = this.ItemId,
                    UpdateTime = this.UpdateTime,
                    Unknown1 = this.Unknown1,
                    OrderInList = this.OrderInList,
                    Quality = this.Quality,
                    Unknown2 = this.Unknown2,
                    Materia1 = this.Materia1,
                    Materia2 = this.Materia2,
                    Materia3 = this.Materia3,
                    Materia4 = this.Materia4,
                    Materia5 = this.Materia5,
                    Unknown3 = this.Unknown3,
                    Unknown4 = this.Unknown4,
                    RetainerName = new string((sbyte*)p, 0, 32, Encoding.UTF8).Split('\0')[0],
                    PlayerName = new string((sbyte*)q, 0, 32, Encoding.UTF8).Split('\0')[0],
                    IsHq = this.IsHq,
                    MateriaCount = this.MateriaCount,
                    OnMannequin = this.OnMannequin,
                    RetainerLocation = this.RetainerLocation,
                    Unknown5 = this.Unknown5,
                    Unknown6 = this.Unknown6,
                };
            }
        }
    }
}
