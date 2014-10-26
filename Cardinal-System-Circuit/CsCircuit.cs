using System.Net;

namespace Cardinal_System_Circuit
{
    public class CsCircuit
    {
        private readonly CsCircuitConnector _connector;

        public CsCircuit(int port)
        {
            _connector = new CsCircuitConnector(IPAddress.Parse("127.0.0.1"), port);
        }

        public void StartTest()
        {
            _connector.RegisterComponentAddress(2, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25252));
            _connector.RegisterComponentInterestInEntity(2, 2);
            Start();
        }

        public void Start()
        {
            _connector.Start();
        }

    }
}