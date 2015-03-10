using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;
using Cardinal_System_Shared.Dto;

namespace Cardinal_System_Common
{
    public class ComponentConnection
    {
        private readonly ConcurrentQueue<MessageDto> SenderQueue;
        private readonly ConcurrentQueue<MessageDto> ReceiverQueue;
        private readonly TcpClient _client;
        private readonly object _hasDiconnectedLock = new object();
        private Task _receiveMessageLoop;
        private bool _hasDisconnected;
        private MessageSender _messageSender;
        private MessageListener _messageListener;
        private readonly ManualResetEventSlim _manualResetEventSlimSender = new ManualResetEventSlim();
        private readonly ManualResetEventSlim _manualResetEventSlimReceiving = new ManualResetEventSlim();

        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public bool IsConnected
        {
            get { return !_hasDisconnected; }
        }

        public ComponentConnection(string initialAddress, int port)
        {
            IpAddress = initialAddress;
            Port = port;
            _hasDisconnected = false;
            _client = new TcpClient();
            ReceiverQueue = new ConcurrentQueue<MessageDto>();
            SenderQueue = new ConcurrentQueue<MessageDto>();
            _receiveMessageLoop = Task.Factory.StartNew(ReceiveMessageLoop);
        }

        public ComponentConnection(TcpClient client)
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
                        if (dto.TargetComponent == 0)
                        {
                            var message = dto.TranslateFromDto();
                            var wrappedMessage = new WrappedMessage(this, message);
                            if (message.Type == MessageType.HeathCliffNewIdRequest) // TODO: Make HC specific [1/3]
                            {
                                HeathCliffNewIdRequest request = message as HeathCliffNewIdRequest;
                                IpAddress = request.IpAddress;
                                Port = request.Port;
                            }
                            MessageHubV2.Send(wrappedMessage);
                        }
                        else if (dto.TargetComponent == ComponentSettings.ComponentId)
                        {
                            var message = dto.TranslateFromDto();
                            MessageHubV2.Send(message);
                        }
                        else
                        {
                            //Forward to intended target
                            MessageHubV2.Send(dto);
                        }
                    }
                }
                _manualResetEventSlimReceiving.Wait(TimeSpan.FromSeconds(1));
                _manualResetEventSlimReceiving.Reset();
            }
        }

        public void Start()
        {
            if (!_client.Connected)
                _client.Connect(IpAddress, Port);

            if (_client.Connected)
            {
                _messageSender = new MessageSender(_client, SenderQueue, HasDisconnected, _manualResetEventSlimSender);
                _messageListener = new MessageListener(_client, ReceiverQueue, HasDisconnected, _manualResetEventSlimReceiving);
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
            _manualResetEventSlimSender.Set();
        }

        public void SendMessage(Message message)
        {
            message.SourceComponent = ComponentSettings.ComponentId;
            SenderQueue.Enqueue(message.ToDto());
            _manualResetEventSlimSender.Set();
        }

        public void SendMessageDto(MessageDto messageDto)
        {
            SenderQueue.Enqueue(messageDto);
        }
    }
}