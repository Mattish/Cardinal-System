using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Dto;

namespace Cardinal_System_Common
{
    public class CsComponentConnection
    {
        private readonly IPAddress _initialAddress;
        private readonly int _initialPort;
        private readonly ConcurrentQueue<MessageDto> SenderQueue;
        private readonly ConcurrentQueue<MessageDto> ReceiverQueue;
        private readonly TcpClient _client;
        private readonly object _hasDiconnectedLock = new object();
        private Task _receiveMessageLoop;
        private bool _hasDisconnected;
        private CsMessageSender _messageSender;
        private CsMessageListener _messageListener;

        public long Id { get; private set; }

        public CsComponentConnection(string initialAddress, int initialPort)
        {
            _initialAddress = IPAddress.Parse(initialAddress);
            _initialPort = initialPort;
            _hasDisconnected = false;
            _client = new TcpClient();
            ReceiverQueue = new ConcurrentQueue<MessageDto>();
            SenderQueue = new ConcurrentQueue<MessageDto>();
            _receiveMessageLoop = Task.Factory.StartNew(ReceiveMessageLoop);
        }

        public CsComponentConnection(TcpClient client)
        {
            _hasDisconnected = false;
            _client = client;
            ReceiverQueue = new ConcurrentQueue<MessageDto>();
            SenderQueue = new ConcurrentQueue<MessageDto>();
            _receiveMessageLoop = Task.Factory.StartNew(ReceiveMessageLoop);
            Start();
        }

        private void ReceiveMessageLoop()
        {
            while (!_hasDisconnected)
            {
                while (!ReceiverQueue.IsEmpty)
                {
                    MessageDto dto;
                    if (ReceiverQueue.TryDequeue(out dto))
                    {
                        Message message = dto.TranslateFromDto();
                        if (message.SourceComponent == 0)
                        {
                            var wrappedMessage = new WrappedMessage(this, message);
                            MessageHubV2.Send(wrappedMessage);
                        }
                        else
                        {
                            MessageHubV2.Send(message);
                        }
                    }
                }
                Thread.Sleep(1); //TODO: signal for message
            }
        }

        public void Start()
        {
            if (!_client.Connected)
                _client.Connect(_initialAddress, _initialPort);

            if (_client.Connected)
            {
                _messageSender = new CsMessageSender(_client, SenderQueue, HasDisconnected);
                _messageListener = new CsMessageListener(_client, ReceiverQueue, HasDisconnected);
                _messageListener.Start();
                _messageSender.Start();
            }
        }

        public void Stop()
        {
            _client.Close();
            lock (_hasDiconnectedLock)
            {
                _hasDisconnected = true;
            }
        }

        private void HasDisconnected()
        {
            lock (_hasDiconnectedLock)
            {
                if (_hasDisconnected)
                {
                    MessageHubV2.Send(new ComponentConnectionDisconnect(this));
                }
                _hasDisconnected = true;
            }
        }

        public void SendMessages(IEnumerable<MessageDto> messages)
        {
            foreach (var messageDto in messages)
            {
                SenderQueue.Enqueue(messageDto);
            }
        }

        public void SendMessage(Message message)
        {
            SenderQueue.Enqueue(message.ToDto());
        }
    }
}