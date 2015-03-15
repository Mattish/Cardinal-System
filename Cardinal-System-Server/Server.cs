using System.Collections.Concurrent;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Server
{
    public class Server : Node
    {
        public Area Area { get; private set; }
        public Heartbeater Heartbeater { get; private set; }
        private readonly ConcurrentDictionary<EntityId, Entity> _entities;

        public Server(Area intialArea, string ipAddress, int port)
            : base(ipAddress, port)
        {
            Area = intialArea;
            _entities = new ConcurrentDictionary<EntityId, Entity>();
        }

        protected override void ComponentSpecificMessageRegisters()
        {
        }
    }
}