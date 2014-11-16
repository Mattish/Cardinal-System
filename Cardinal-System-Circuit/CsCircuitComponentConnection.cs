using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Dtos;

namespace Cardinal_System_Circuit
{
    public class CsCircuitComponentConnection
    {
        private readonly CsMessageListener _circuitListener;
        private readonly CsMessageSender _circuitSender;
        private readonly ConcurrentQueue<MessageDto> _senderQueue;
        private readonly ConcurrentQueue<MessageDto> _receiverQueue;
        private readonly TcpClient _client;
        public CsCircuitComponentConnection(TcpClient client)
        {
            _client = client;
            _senderQueue = new ConcurrentQueue<MessageDto>();
            _receiverQueue = new ConcurrentQueue<MessageDto>();
            _circuitListener = new CsMessageListener(client, _receiverQueue);
            _circuitSender = new CsMessageSender(client, _senderQueue);
        }

        public ConcurrentQueue<MessageDto> ReceiverQueue
        {
            get { return _receiverQueue; }
        }

        public ConcurrentQueue<MessageDto> SenderQueue
        {
            get { return _senderQueue; }
        }

        public bool isConnected
        {
            get
            {
                return _client.Connected;
            }
        }

        public void Start()
        {
            _circuitListener.Start();
            _circuitSender.Start();
        }

        public bool Clearup()
        {
            while (!_circuitListener.IsRunning && !_circuitSender.IsRunning)
            {
                
            }
            return true;
        }
    }
}