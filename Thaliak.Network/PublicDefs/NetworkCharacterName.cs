using System.Runtime.InteropServices;
using System.Text;

namespace Thaliak.Network
{
    public class NetworkCharacterName : NetworkMessage
    {
        public long CharacterId;
        public string Name;

        public new static int GetMessageId()
        {
            return 0x018E;
        }

        public new static unsafe NetworkCharacterName Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkCharacterNameRaw*) raw).Spawn();
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct NetworkCharacterNameRaw : INetworkMessageBase<NetworkCharacterName>
    {
        [FieldOffset(0)]
        public long CharacterId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        [FieldOffset(8)]
        public fixed byte Name[32];

        public NetworkCharacterName Spawn()
        {
            fixed (byte* p = this.Name)
            {
                return new NetworkCharacterName
                {
                    CharacterId = this.CharacterId,
                    Name = new string((sbyte*)p, 0, 32, Encoding.UTF8).Split('\0')[0],
                };
            }
        }
    }
}
