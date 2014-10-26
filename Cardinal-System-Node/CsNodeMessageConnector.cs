using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    class CsNodeMessageConnector
    {
        private readonly IPAddress _circuitAddress;
        private readonly int _circuitPort;
        private readonly int _listeningPort;
        private CsNodeMessageSender _messageSender;
        private CsNodeMessageListener _messageListener;
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _senderQueue;
        private readonly ConcurrentQueue<MessageDto> _receiverQueue;

        public CsNodeMessageConnector(int listeningPort, IPAddress circuitAddress, int circuitPort, ConcurrentDictionary<int, Entity> entities)
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
                _messageSender = new CsNodeMessageSender(_client, _senderQueue);
                _messageListener = new CsNodeMessageListener(_client, _receiverQueue);
                _messageListener.Start();
                _messageSender.Start();

            }
        }

        public void StartTest()
        {
            var list = new List<Message>(2500);
            for (int i = 0; i < 250; i++)
            {
                var entity = new PhysicalEntity();
                for (int j = 0; j < 10; j++)
                {
                    list.Add(new PhysicalMovementMessage
                    {
                        SourceId = 0,
                        TargetId = 2,
                        NewPosition = new Tuple<int, int>(i, 1)
                    });
                }
                Thread.Sleep(1);
            }
            foreach (var message in list)
            {
                var messageDto = new MessageDto
                {
                    Type = message.GetMessageType(),
                    SourceId = 1,
                    TargetId = 2,
                    Message = JsonConvert.SerializeObject(message)
                };
                _senderQueue.Enqueue(messageDto);
            }
            Console.WriteLine("Enqueue {0} messages", _senderQueue.Count);
            list.Clear();
            Start();
            Thread.Sleep(2500);
            for (int i = 0; i < 250; i++)
            {
                var entity = new PhysicalEntity();
                for (int j = 0; j < 10; j++)
                {
                    list.Add(new PhysicalMovementMessage
                    {
                        SourceId = 0,
                        TargetId = 2,
                        NewPosition = new Tuple<int, int>(i, 1)
                    });
                }
                Thread.Sleep(1);
            }
            foreach (var message in list)
            {
                var messageDto = new MessageDto
                {
                    Type = message.GetMessageType(),
                    SourceId = 1,
                    TargetId = 2,
                    Message = JsonConvert.SerializeObject(message)
                };
                _senderQueue.Enqueue(messageDto);
            }
            Console.WriteLine("Enqueue {0} messages", _senderQueue.Count);
        }
    }
}