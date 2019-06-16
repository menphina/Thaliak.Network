namespace Thaliak.Network
{
    public abstract class NetworkMessage
    {
        public static int GetMessageId()
        {
            return -1;
        }

        public static unsafe NetworkMessage Consume(byte[] data, int offset)
        {
            return default;
        }
    }
}
