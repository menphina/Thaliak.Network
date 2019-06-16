using System.Runtime.InteropServices;

namespace Thaliak.Network
{
    public class NetworkMarketListingCount : NetworkMessage
    {
        public int ItemId;
        public int Unknown1;
        public short RequestId;
        public short Quantity;
        public int Unknown2;

        public new static int GetMessageId()
        {
            return 0x0125;
        }

        public new static unsafe NetworkMarketListingCount Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkMarketListingCountRaw*) raw).Spawn();
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkMarketListingCountRaw : INetworkMessageBase<NetworkMarketListingCount>
    {
        [FieldOffset(0)]
        public int ItemId;

        [FieldOffset(4)]
        public int Unknown1;

        [FieldOffset(8)]
        public short RequestId;

        [FieldOffset(10)]
        public fixed byte Quantity[2]; // This field is big-endian integer

        [FieldOffset(12)]
        public int Unknown2;

        public NetworkMarketListingCount Spawn()
        {
            return new NetworkMarketListingCount
            {
                ItemId = this.ItemId,
                Unknown1 = this.Unknown1,
                RequestId = this.RequestId,
                Quantity = (short)((this.Quantity[0] << 8) | this.Quantity[1]),
                Unknown2 = this.Unknown2,
            };
        }
    }
}
