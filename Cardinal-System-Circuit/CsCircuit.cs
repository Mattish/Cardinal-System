using System;
using System.Configuration;
using System.Net;
using Cardinal_System_Circuit.InternalMessages;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;

namespace Cardinal_System_Circuit
{
    public class CsCircuit : Getter<Message>, ICsNode
    {
        private readonly CsCircuitConnector _connector;
        private readonly string _address;
        private readonly int _port;

        public CsCircuit(string address, int port)
        {
            _address = address;
            _port = port;
            _connector = new CsCircuitConnector(address, port);
        }

        public void Start()
        {
            _connector.Start();
            MessageHubV2.Send(new ConnectToHeathCliffRequest(ConfigurationManager.AppSettings["HCAddress"], int.Parse(ConfigurationManager.AppSettings["HCPort"])));
        }

        protected override void SpecificAction(Message request)
        {
            Console.WriteLine("CsCircuit - SpecificAction - {0}", request);
        }
    }
}