using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Cardinal_System_Common;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Node
{
    internal class CsNode
    {
        public static long Identity { get; private set; }
        public static CsArea Area { get; private set; }

        private readonly int _circuitPort;
        private readonly int _listeningPort;
        private readonly CsNodeMessageConnector _messageConnector;
        private readonly ConcurrentDictionary<long, Entity> _entities;

        public CsNode(long identity, CsArea intialArea, IPAddress circuitAddress, int circuitPort, int listeningPort)
        {
            Identity = identity;
            Area = intialArea;
            _circuitPort = circuitPort;
            _listeningPort = listeningPort;
            _entities = new ConcurrentDictionary<long, Entity>();
            _messageConnector = new CsNodeMessageConnector(listeningPort, circuitAddress, circuitPort, _entities);
        }

        public void Start()
        {
            _messageConnector.Start();
            _messageConnector.SendInfo();
        }

        public void SendRegister(Tuple<long, long> entityNumber)
        {
            _messageConnector.SendRegister(entityNumber);
        }

        public void Stop()
        {
            _messageConnector.Stop();
        }

        public void CreatePhysicalEntity()
        {
            var newEntity = new PhysicalEntity();
            _entities.TryAdd(newEntity.GlobalId, newEntity);
            _messageConnector.SendNewEntity(newEntity);
        }
    }
}