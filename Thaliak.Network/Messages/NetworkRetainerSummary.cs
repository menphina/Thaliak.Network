using Milvaneth.Common;
using System.Runtime.InteropServices;
using Thaliak.Network.Utilities;

namespace Thaliak.Network.Messages
{
    public class NetworkRetainerSummary : NetworkMessage
    {
        public int Unknown1;
        public int Unknown2; // BE 7F 00 00
        public long RetainerId; //?
        public byte RetainerOrder;
        public byte Unknown3; // BC
        public short Unknown4; // 69 01
        public byte ItemsInSell;
        public byte Unknown5; // 00
        public short Unknown6; // 00 00
        public byte Unknown7; // 00/01
        public byte RetainerLocation;
        public short Unknown8; // 6B 01
        public int ListingDueDate; // unix, due may = 0?
        public short Unknown9; // 01/00 00
        public string RetainerName; // 32b
        public short Unknown10; // B2 EC
        public int Unknown11; // BD 7F 00 00

        public new static int GetMessageId()
        {
            return MessageIdRetriver.Instance.GetMessageId(MessageIdRetriveKey.NetworkRetainerSummary);
        }

        public new static unsafe NetworkRetainerSummary Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkRetainerSummaryRaw*)raw).Spawn(data, offset);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkRetainerSummaryRaw : INetworkMessageBase<NetworkRetainerSummary>
    {
        [FieldOffset(0)]
        public int Unknown1;

        [FieldOffset(4)]
        public int Unknown2;

        [FieldOffset(8)]
        public long RetainerId;

        [FieldOffset(16)]
        public byte RetainerOrder;

        [FieldOffset(17)]
        public byte Unknown3;

        [FieldOffset(18)]
        public short Unknown4;

        [FieldOffset(20)]
        public byte ItemsInSell;

        [FieldOffset(21)]
        public byte Unknown5;

        [FieldOffset(22)]
        public short Unknown6;

        [FieldOffset(24)]
        public byte Unknown7;

        [FieldOffset(25)]
        public byte RetainerLocation;

        [FieldOffset(26)]
        public short Unknown8;

        [FieldOffset(28)]
        public int ListingDueDate;

        [FieldOffset(32)]
        public short Unknown9;

        [FieldOffset(66)]
        public short Unknown10;

        [FieldOffset(68)]
        public int Unknown11;

        public NetworkRetainerSummary Spawn(byte[] data, int offset)
        {
            return new NetworkRetainerSummary
            {
                Unknown1 = this.Unknown1,
                Unknown2 = this.Unknown2,
                RetainerId = this.RetainerId,
                RetainerOrder = this.RetainerOrder,
                Unknown3 = this.Unknown3,
                Unknown4 = this.Unknown4,
                ItemsInSell = this.ItemsInSell,
                Unknown5 = this.Unknown5,
                Unknown6 = this.Unknown6,
                Unknown7 = this.Unknown7,
                RetainerLocation = this.RetainerLocation,
                Unknown8 = this.Unknown8,
                ListingDueDate = this.ListingDueDate,
                Unknown9 = this.Unknown9,
                Unknown10 = this.Unknown10,
                Unknown11 = this.Unknown11,
                RetainerName = Helper.ToUtf8String(data, offset, 34, 32),
            };
        }
    }
}
