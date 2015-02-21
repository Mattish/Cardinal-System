using System;
using System.Collections.Concurrent;
using System.Net;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Server
{
    public class CsServer : Getter<Message>, ICsNode
    {
        public static CsArea Area { get; private set; }

        private readonly CsServerConnector _messageConnector;
        private readonly ConcurrentDictionary<EntityId, Entity> _entities;

        public CsServer(CsArea intialArea, string circuitAddress, int circuitPort)
        {
            Area = intialArea;
            _entities = new ConcurrentDictionary<EntityId, Entity>();
            _messageConnector = new CsServerConnector(circuitAddress, circuitPort);
        }

        private static void Disconnected(CsComponentConnection connection)
        {
            //TODO: do me
        }

        public void Start()
        {
            _messageConnector.Start();
        }

        protected override void SpecificAction(Message request)
        {
            Console.WriteLine("CsServer - SpecificAction - {0}", request);
        }

        public void Stop()
        {
            _messageConnector.Stop();
        }
    }
}
