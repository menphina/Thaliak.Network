using System;

namespace Thaliak.Network.Dispatcher
{
    class RoutedMessage
    {
        public Type Consumer { get; }
        public MessageDecoded Listener { get; }
        public int HeaderLength { get; }
        public byte[] Message { get; }
        public NetworkMessageHeader Header { get; }

        public RoutedMessage(NetworkMessageHeader header, int len, byte[] msg, Type consumer, MessageDecoded listener)
        {
            Header = header;
            HeaderLength = len;
            Message = msg;
            Consumer = consumer;
            Listener = listener;
        }
    }
}
