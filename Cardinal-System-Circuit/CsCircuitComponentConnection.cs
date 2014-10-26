using System.Collections.Concurrent;
using System.Net.Sockets;
using Cardinal_System_Common;
using Cardinal_System_Shared;

namespace Cardinal_System_Circuit
{
    public class CsCircuitComponentConnection
    {
        private readonly CsMessageListener _circuitListener;
        private readonly CsMessageSender _circuitSender;
        private readonly ConcurrentQueue<MessageDto> _senderQueue;
        public CsCircuitComponentConnection(TcpClient client, ConcurrentQueue<MessageDto> receivingQueue)
        {
            _senderQueue = new ConcurrentQueue<MessageDto>();
            _circuitListener = new CsMessageListener(client, receivingQueue);
            _circuitSender = new CsMessageSender(client, _senderQueue);
        }

        public ConcurrentQueue<MessageDto> GetSenderQueue
        {
            get { return _senderQueue; }
        }

        public void Start()
        {
            _circuitListener.Start();
            _circuitSender.Start();
        }
    }
}