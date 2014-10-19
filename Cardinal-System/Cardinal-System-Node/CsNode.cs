using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNode
    {
        private readonly int _port;
        private readonly CsNodeEntityChangeSender _entityChangeSender;
        private readonly CsNodeEntityChangeReceiver _entityChangeReceiver;
        private readonly ConcurrentDictionary<int, Entity> entities;

        public CsNode(int port)
        {
            _port = port;
            entities = new ConcurrentDictionary<int, Entity>();
            _entityChangeSender = new CsNodeEntityChangeSender();
            _entityChangeReceiver = new CsNodeEntityChangeReceiver(port, entities);
        }

        public void StartTest()
        {
            _entityChangeReceiver.Start();
            var list = new List<EntityChange>(200);
            for (int i = 0; i < 250; i++)
            {
                var entity = new PhysicalEntity();
                for (int j = 0; j < 250; j++)
                {
                    list.Add(new PhysicalMovementEntityChange { PositionChange = new Tuple<int, int>(i, 1) });
                }

                _entityChangeSender.SendMany(list, "127.0.0.1", _port);
                list.Clear();
                Thread.Sleep(20);
            }
        }

        public void Start()
        {
            _entityChangeReceiver.Start();
            var list = new List<EntityChange>(200);
            for (int i = 0; i < 1000; i++)
            {
                var entity = new PhysicalEntity();
                list.Add(new PhysicalCreateEntityChange(entity.GlobalId, entity.Position));
            }
            DateTime lastNow = DateTime.UtcNow;
            while (true)
            {
                for (int i = 0; i < 1000; i++)
                {
                    list.Add(new PhysicalMovementEntityChange { PositionChange = new Tuple<int, int>(i, 1) });
                }
                _entityChangeSender.SendMany(list, "127.0.0.1", _port);
                list.Clear();
                TimeSpan timeToWait = DateTime.UtcNow - lastNow;
                timeToWait = timeToWait > TimeSpan.FromMilliseconds(100) ? TimeSpan.Zero : TimeSpan.FromMilliseconds(100) - timeToWait;
                Console.WriteLine("Waiting {0}ms", timeToWait.TotalMinutes);
                lastNow = DateTime.UtcNow;
            }
        }
    }
}