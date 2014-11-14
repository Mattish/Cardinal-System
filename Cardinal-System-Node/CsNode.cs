using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNode
    {
        private readonly int _circuitPort;
        private readonly int _listeningPort;
        private readonly CsNodeMessageConnector _messageConnector;
        private readonly ConcurrentDictionary<int, Entity> _entities;

        public CsNode(long identity, IPAddress circuitAddress, int circuitPort, int listeningPort)
        {
            Identity = identity;
            _circuitPort = circuitPort;
            _listeningPort = listeningPort;
            _entities = new ConcurrentDictionary<int, Entity>();
            _messageConnector = new CsNodeMessageConnector(listeningPort, IPAddress.Parse("127.0.0.1"), circuitPort, _entities);
        }

        public void Start()
        {
            _messageConnector.Start();
            _messageConnector.SendInfo(Identity);
        }

        public void SendRegister(long entityNumber)
        {
            _messageConnector.SendRegister(entityNumber);
        }

        public void Stop()
        {
            _messageConnector.Stop();
        }

        public static long Identity { get; private set; }
    }
}