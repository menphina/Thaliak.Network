using System.Runtime.InteropServices;

namespace Thaliak.Network
{
    public class NetworkPlayerSpawn : NetworkMessage
    {
        public int CurrentWorldId;
        public int HomeWorldId;

        public new static int GetMessageId()
        {
            return 0x0175;
        }

        public new static unsafe NetworkPlayerSpawn Consume(byte[] data, int offset)
        {
            fixed (byte* raw = &data[offset])
            {
                return (*(NetworkPlayerSpawnRaw*) raw).Spawn();
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct NetworkPlayerSpawnRaw : INetworkMessageBase<NetworkPlayerSpawn>
    {
        [FieldOffset(4)]
        public int CurrentWorldId;

        [FieldOffset(6)]
        public int HomeWorldId;

        public NetworkPlayerSpawn Spawn()
        {
            return new NetworkPlayerSpawn
            {
                CurrentWorldId = this.CurrentWorldId,
                HomeWorldId = this.HomeWorldId,
            };
        }
    }
}
