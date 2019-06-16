using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thaliak.Network.Analyzer;

namespace Thaliak.Network.Dispatcher
{
    public class MessageDispatcher : IAnalyzerOutput
    {
        // length field is not included in Message, so each place involves offsets should minus 4
        private const int HeaderLength = 32;
        private const int MsgTypeOffset = 18;
        private const int TimestampOffset = 24;
        private readonly Dictionary<int, Type> _consumerTypes;
        private readonly Dictionary<int, MessageDecoded> _listeners;

        public delegate void MessageDecoded(NetworkMessageHeader header, NetworkMessage message);

        public MessageDispatcher(IEnumerable<Type> consumers)
        {
            _consumerTypes = new Dictionary<int, Type>();
            _listeners = new Dictionary<int, MessageDecoded>();

            foreach (var consumer in consumers)
            {
                if(!consumer.IsSubclassOf(typeof(NetworkMessage)))
                    throw new InvalidOperationException($"Not inherited from {nameof(NetworkMessage)}");

                var method = consumer.GetMethod(nameof(NetworkMessage.GetMessageId));

                var opCode = (int)method.Invoke(null, new object[] { });
                _consumerTypes.Add(opCode, consumer);
            }
        }

        public unsafe void Output(AnalyzedPacket analyzedPacket)
        {
            if (analyzedPacket.Message.Length < HeaderLength) return;
            
            // for default filter set we use, every single filter is mutually exclusive.
            var direction = analyzedPacket.RouteMark.First().GetDirection();
            //if (direction != MessageAttribute.DirectionReceive) return;

            NetworkMessageHeader header;
            fixed (byte* p = &analyzedPacket.Message[0])
            {
                header = *(NetworkMessageHeader*) p;
            }

            if (!_consumerTypes.TryGetValue(header.OpCode, out var consumer)) return;
            if (!_listeners.TryGetValue(header.OpCode, out var listener) || listener == null) return;

            var output = (NetworkMessage)consumer.GetMethod(nameof(NetworkMessage.Consume)).Invoke(null, new object[] {analyzedPacket.Message, HeaderLength});

            listener(header, output);
        }

        public void Subcribe(int opcode, MessageDecoded listener)
        {
            if (!_listeners.ContainsKey(opcode))
                _listeners.Add(opcode, null);

            if (listener != null) _listeners[opcode] += listener;
        }

        public void Unsubcribe(int opcode, MessageDecoded listener)
        {
            if (!_listeners.ContainsKey(opcode))
                return;

            if (listener != null) _listeners[opcode] -= listener;

            if (_listeners[opcode] == null) _listeners.Remove(opcode);
        }
    }
}
