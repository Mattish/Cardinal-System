using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNode
    {
        private readonly int _port;
        private readonly CsNodeMessageSender _messageSender;
        private readonly CsNodeMessageReceiver _messageReceiver;
        private readonly ConcurrentDictionary<int, Entity> entities;

        public CsNode(int port)
        {
            _port = port;
            entities = new ConcurrentDictionary<int, Entity>();
            _messageSender = new CsNodeMessageSender();
            _messageReceiver = new CsNodeMessageReceiver(port, entities);
        }

        public void StartTest(string ipAddress, int port)
        {
            _messageReceiver.Start();
            var list = new List<Message>(200);
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

                _messageSender.SendMany(list, ipAddress, port);
                list.Clear();
                Thread.Sleep(10);
            }
        }

        public void Start()
        {
            _messageReceiver.Start();
        }
    }
}