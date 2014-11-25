using System;
using System.Collections.Concurrent;
using System.Net;
using Cardinal_System_Common;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Server
{
    public class CsServer : CsNode
    {
        public static CsArea Area { get; private set; }

        private readonly CsServerConnector _messageConnector;
        private readonly ConcurrentDictionary<EntityId, Entity> _entities;

        public CsServer(CsArea intialArea, IPAddress circuitAddress, int circuitPort)
        {
            Area = intialArea;
            _entities = new ConcurrentDictionary<EntityId, Entity>();
            _messageConnector = new CsServerConnector(circuitAddress, circuitPort, Disconnected);
        }

        private static void Disconnected(CsComponentConnection connection)
        {
            //TODO: do me
        }

        public override bool IsRunning
        {
            //TODO do me
            get { return true; }
        }

        public override void Start()
        {
            _messageConnector.Start();
            _messageConnector.SendInfo();
        }

        public void SendRegister(EntityId entityNumber)
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
            _entities.TryAdd(newEntity.Id, newEntity);
            _messageConnector.SendNewEntity(newEntity);
        }
    }
}
