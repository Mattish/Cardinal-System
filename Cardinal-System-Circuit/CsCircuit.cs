using System.Collections.Concurrent;
using System.Net;
using Cardinal_System_Shared;

namespace Cardinal_System_Circuit
{
    public class CsCircuit
    {
        private readonly CsCircuitListener _circuitListener;
        private readonly CsCircuitSender _circuitSender;

        public CsCircuit(int port)
        {
            var receivingQueue = new ConcurrentQueue<MessageDto>();
            _circuitListener = new CsCircuitListener(port, receivingQueue);
            _circuitSender = new CsCircuitSender(receivingQueue);
        }

        public void StartTest()
        {
            RegisterComponentAddress(2, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25252));
            RegisterComponentInterestInEntity(2, 2);
            Start();
        }

        public void Start()
        {
            _circuitListener.Start();
            _circuitSender.Start();
        }


        public void RegisterComponentAddress(int componentId, IPEndPoint endpoint)
        {
            _circuitSender.RegisterComponentAddress(componentId, endpoint);
        }

        public void RegisterComponentInterestInEntity(int componentId, int entityId)
        {
            _circuitSender.RegisterComponentInterestInEntity(componentId, entityId);
        }

    }
}