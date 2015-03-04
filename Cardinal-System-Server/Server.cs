using System;
using System.Collections.Concurrent;
using System.Net;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Server
{
    public class Server : Getter<Message>, ICsNode
    {
        public static Area Area { get; private set; }

        private readonly ServerConnector _messageConnector;
        private readonly ConcurrentDictionary<EntityId, Entity> _entities;

        public Server(Area intialArea, string circuitAddress, int circuitPort)
        {
            Area = intialArea;
            _entities = new ConcurrentDictionary<EntityId, Entity>();
            _messageConnector = new ServerConnector(circuitAddress, circuitPort);
        }

        private static void Disconnected(ComponentConnection connection)
        {
            //TODO: do me
        }

        public void Start()
        {
            _messageConnector.Start();
        }

        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("Server - SpecificAction - {0}", message);
        }

        public void Stop()
        {
            _messageConnector.Stop();
        }
    }
}
