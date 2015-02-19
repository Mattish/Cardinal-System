using System;
using System.Net;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared.Dtos;

namespace Cardinal_System_Circuit
{
    public class CsCircuit : Getter<Message>, ICsNode
    {
        private readonly CsCircuitConnector _connector;
        private readonly int _port;

        public CsCircuit(string address, int port)
        {
            _port = port;
            _connector = new CsCircuitConnector(address, port);
        }

        public void Start()
        {
            _connector.Start();
        }

        protected override void SpecificAction(Message request)
        {
            Console.WriteLine("CsCircuit - SpecificAction - {0}", request);
        }
    }
}