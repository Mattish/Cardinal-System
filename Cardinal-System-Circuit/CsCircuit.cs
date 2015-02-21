using System;
using System.Configuration;
using System.Net;
using Cardinal_System_Circuit.InternalMessages;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;

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

        protected override void ExtraMessageRegisters()
        {
            MessageHubV2.Register<WrappedMessage>(this, HandleWrappedMessage);
        }

        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("{0} SourceComponentId:{1}", message, message.SourceComponent);
            if (message.Type == MessageType.HeathCliffNewIdResponse)
            {
                var newComponentId = ((HeathCliffNewIdResponse)message).NewId;
                Console.WriteLine("{0} - {1}", message, newComponentId);
                CsComponentSettings.ComponentId = newComponentId;
                MessageHubV2.Send(new HeartbeatMessage
                {
                    ComponentId = -1
                });
            }
        }

        private void HandleWrappedMessage(WrappedMessage wrappedMessage)
        {
            Console.WriteLine("{0} SourceComponentId:{1}", wrappedMessage.Message, wrappedMessage.Message.SourceComponent);

        }
    }
}