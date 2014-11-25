using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared.Dtos;

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

        protected readonly Action<MessageDto, CsComponentConnection> GotMessageAction;
        protected readonly Action<CsComponentConnection> DisconnectAction;

        public CsComponentConnection(IPAddress initialAddress, int initialPort, Action<MessageDto, CsComponentConnection> gotMessageAction, Action<CsComponentConnection> disconnectAction)
        {
            _initialAddress = initialAddress;
            _initialPort = initialPort;
            GotMessageAction = gotMessageAction;
            DisconnectAction = disconnectAction;
            _hasDisconnected = false;
            _client = new TcpClient();
            ReceiverQueue = new ConcurrentQueue<MessageDto>();
            SenderQueue = new ConcurrentQueue<MessageDto>();
            _receiveMessageLoop = Task.Factory.StartNew(ReceiveMessageLoop);
        }

        public CsComponentConnection(TcpClient client, Action<MessageDto, CsComponentConnection> gotMessageAction, Action<CsComponentConnection> disconnectAction)
        {
            GotMessageAction = gotMessageAction;
            DisconnectAction = disconnectAction;
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
                        GotMessageAction(dto, this);
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
                    DisconnectAction(this);
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

        public void SendMessage(MessageDto message)
        {
            SenderQueue.Enqueue(message);
        }
    }
}