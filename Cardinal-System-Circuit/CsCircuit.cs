using System;
using System.Net;
using Cardinal_System_Common;

namespace Cardinal_System_Circuit
{
    public class CsCircuit : CsNode
    {
        private readonly CsCircuitConnector _connector;
        private readonly int _port;

        public CsCircuit(int port)
        {
            _port = port;
            _connector = new CsCircuitConnector(IPAddress.Parse("127.0.0.1"), port);
        }

        public override bool IsRunning
        {
            //TODO do me
            get { return true; }
        }

        public override void Start()
        {
            _connector.Start();
            Console.WriteLine("Listening on port {0}", _port);
        }
    }
}