using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cardinal_System_Common;
using Cardinal_System_Shared.Dtos;
using Cardinal_System_Shared.Dtos.Component;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Node
{
    class CsNodeMessageConnector
    {
        private readonly IPAddress _circuitAddress;
        private readonly int _circuitPort;
        private readonly int _listeningPort;
        private CsMessageSender _messageSender;
        private CsMessageListener _messageListener;
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _senderQueue;
        private readonly ConcurrentQueue<MessageDto> _receiverQueue;

        public CsNodeMessageConnector(int listeningPort, IPAddress circuitAddress, int circuitPort, ConcurrentDictionary<EntityId, Entity> entities)
        {
            _senderQueue = new ConcurrentQueue<MessageDto>();
            _receiverQueue = new ConcurrentQueue<MessageDto>();
            _circuitAddress = circuitAddress;
            _circuitPort = circuitPort;
            _listeningPort = listeningPort;
            _client = new TcpClient();
        }

        public void Start()
        {
            _client.Connect(_circuitAddress, _circuitPort);
            if (_client.Connected)
            {
                _messageSender = new CsMessageSender(_client, _senderQueue);
                _messageListener = new CsMessageListener(_client, _receiverQueue);
                _messageListener.Start();
                _messageSender.Start();

            }
        }

        public void StartTest2()
        {
            Start();
            _senderQueue.Enqueue(new RegisterEntityInterestMessage
            {
                SourceId = new EntityId(CsNode.Identity, 10),
                TargetId = new EntityId(CsNode.Identity, 69),
                Type = MessageType.RegisterEntityInterest
            }.ToDto());
        }

        public void Stop()
        {
            _client.Close();
            while (_messageSender.IsRunning && _messageListener.IsRunning)
            {
                Console.WriteLine("Waiting for sender and listener to finish up");
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
            Console.WriteLine("sender and listener finished up");
        }

        public void SendInfo()
        {
            _senderQueue.Enqueue(new RegisterWithCircuitMessage
            {
                SourceId = new EntityId(CsNode.Identity, 0)
            }.ToDto());
        }

        public void SendRegister(EntityId entityNumber)
        {
            _senderQueue.Enqueue(new RegisterEntityInterestMessage
            {
                SourceId = new EntityId(CsNode.Identity, 0),
                TargetId = entityNumber
            }.ToDto());
        }

        public void SendNewEntity(PhysicalEntity newEntity)
        {
            _senderQueue.Enqueue(new RegisterEntityOwnerMessage
            {
                SourceId = newEntity.Id,
            }.ToDto());
        }
    }
}