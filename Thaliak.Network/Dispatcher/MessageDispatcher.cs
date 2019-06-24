using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Thaliak.Network.Analyzer;

namespace Thaliak.Network.Dispatcher
{
    public class MessageDispatcher : IAnalyzerOutput
    {
        private const int HeaderLength = 32;

        private bool _isStopping;
        private long _messagesDispatched;

        private readonly BlockingCollection<RoutedMessage> _outputQueue;
        private readonly Dictionary<int, MessageConsumerDelegate> _consumerTypes;
        private readonly Dictionary<int, MessageDecoded> _listeners;

        public long MessagesDispatched => _messagesDispatched;
        public long RequestsInQueue => this._outputQueue.Count;

        public MessageDispatcher(IEnumerable<Type> consumers)
        {
            this._outputQueue = new BlockingCollection<RoutedMessage>();
            _consumerTypes = new Dictionary<int, MessageConsumerDelegate>();
            _listeners = new Dictionary<int, MessageDecoded>();

            foreach (var consumer in consumers)
            {
                if(!consumer.IsSubclassOf(typeof(NetworkMessage)))
                    throw new InvalidOperationException($"Not inherited from {nameof(NetworkMessage)}");

                var method = consumer.GetMethod(nameof(NetworkMessage.GetMessageId));

                var opCode = (int)method.Invoke(null, new object[] { });
                var consume = (MessageConsumerDelegate) Delegate.CreateDelegate(typeof(MessageConsumerDelegate),
                    consumer.GetMethod(nameof(NetworkMessage.Consume)), true);
                _consumerTypes.Add(opCode, consume);
            }
        }

        public unsafe void Output(AnalyzedPacket analyzedPacket)
        {
            if (analyzedPacket.Message.Length < HeaderLength) return;
            
            // for default filter set we use, every single filter is mutually exclusive.
            var direction = analyzedPacket.RouteMark.First().GetDirection();

            NetworkMessageHeader header;
            fixed (byte* p = &analyzedPacket.Message[0])
            {
                header = *(NetworkMessageHeader*) p;
            }

            int opcode = header.OpCode;
            if (direction != MessageAttribute.DirectionReceive) opcode = -opcode;

            if (!_consumerTypes.TryGetValue(opcode, out var consumer)) return;
            if (!_listeners.TryGetValue(opcode, out var listener) || listener == null) return;

            EnqueueOutput(new RoutedMessage(header, HeaderLength, analyzedPacket.Message, consumer, listener));
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var data in this._outputQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        var output = data.Consumer(data.Message, data.HeaderLength);

                        data.Listener(data.Header, output);
                        Interlocked.Increment(ref this._messagesDispatched);
                    }
                    catch
                    {
                    }
                }
            });
        }

        public void Stop()
        {
            this._isStopping = true;
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

        private void EnqueueOutput(RoutedMessage data)
        {
            if (this._isStopping)
            {
                this._outputQueue.CompleteAdding();
                return;
            }

            this._outputQueue.Add(data);
        }
    }
}
