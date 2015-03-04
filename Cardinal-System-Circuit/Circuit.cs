using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Circuit.InternalMessages;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_Circuit
{
    public class Circuit : Getter<Message>, ICsNode
    {
        private readonly CircuitConnector _connector;
        private readonly string _address;
        private readonly int _port;

        public Circuit(string address, int port)
        {
            _address = address;
            _port = port;
            _connector = new CircuitConnector(address, port);
        }

        public void Start()
        {
            _connector.Start();
            ComponentSettings.ComponentType = ComponentType.Circuit;
            MessageHubV2.Send(new ConnectToHeathCliffRequest(ConfigurationManager.AppSettings["HCAddress"], int.Parse(ConfigurationManager.AppSettings["HCPort"])));
        }

        protected override void ExtraMessageRegisters()
        {
            MessageHubV2.Register<WrappedMessage>(this, HandleWrappedMessage);
        }

        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("{0} SourceComponentId:{1} TargetComponentId:{2}", message, message.SourceComponent, message.TargetComponent);
            if (message.Type == MessageType.HeathCliffNewIdResponse)
            {
                var newComponentId = ((HeathCliffNewIdResponse)message).NewId;
                ComponentSettings.ComponentId = newComponentId;
            }
            else if (message.Type == MessageType.HeathCliffOrderConnect)
            {
                var hcOrderConnectMessage = (HeathCliffOrderConnect)message;
                Console.WriteLine("HeathCliffOrderConnect - IpAddress:{0} Port:{1} Id:{2} Type:{3}",
                    hcOrderConnectMessage.IpAddress, hcOrderConnectMessage.Port, hcOrderConnectMessage.ComponentId, hcOrderConnectMessage.ComponentType);

                if (hcOrderConnectMessage.ComponentId != -1)
                {
                    MessageHubV2.Send(new DisconnectFromHeathCliff(hcOrderConnectMessage.IpAddress, hcOrderConnectMessage.Port, hcOrderConnectMessage.ComponentId));
                }
            }
        }

        private void HandleWrappedMessage(WrappedMessage wrappedMessage)
        {
            SpecificAction(wrappedMessage.Message);
        }
    }
}