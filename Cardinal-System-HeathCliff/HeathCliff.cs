using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_HeathCliff
{
    public class HeathCliff : Getter<Message>, ICsNode
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly ConcurrentDictionary<long, ComponentConnection> _componentConnections = new ConcurrentDictionary<long, ComponentConnection>();
        private readonly List<ComponentConnection> _unnumberedComponentConnections = new List<ComponentConnection>(); // TODO: Use a thread-safe collection? [1/3]
        private TcpListener _tcpListener;
        private Task _listenerTask;
        private bool _doProcessing;
        private long _nextComponentId;

        private readonly ComponentManager _manager = new ComponentManager();

        private void DisconnectedComponent(ComponentConnectionDisconnect disconnectedComponentConnection)
        {
            lock (_unnumberedComponentConnections)
            {
                if (_unnumberedComponentConnections.Contains(disconnectedComponentConnection.ComponentConnection))
                {
                    _unnumberedComponentConnections.Remove(disconnectedComponentConnection.ComponentConnection);
                    return;
                }
            }
            var connectionkey = _componentConnections.FirstOrDefault(x => x.Value == disconnectedComponentConnection.ComponentConnection).Key;
            ComponentConnection connection;
            _componentConnections.TryRemove(connectionkey, out connection);
        }

        public HeathCliff(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (_doProcessing)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new ComponentConnection(client); //TODO: When we receive a connection, will it always be unnumbered? [1/3]
                lock (_unnumberedComponentConnections)
                {
                    _unnumberedComponentConnections.Add(componentConnection);
                }
            }
        }

        public void Stop()
        {
            _doProcessing = false;
        }

        public void Start()
        {
            _nextComponentId = 1000;
            _doProcessing = true;
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _listenerTask = Task.Factory.StartNew(DoListening);
        }

        protected override void ExtraMessageRegisters()
        {
            MessageHubV2.Register<WrappedMessage>(this, HandleWrappedMessage);
        }

        private void HandleWrappedMessage(WrappedMessage wrappedMessage)
        {
            Console.WriteLine("{0} - {1} SourceComponentId:{2}", _messageCounter++, wrappedMessage.Message, wrappedMessage.Message.SourceComponent);
            if (wrappedMessage.Message.Type == MessageType.HeathCliffNewIdRequest)
            {
                lock (_unnumberedComponentConnections)
                {
                    var componentId = _nextComponentId++;
                    wrappedMessage.ComponentConnection.SendMessage(new HeathCliffNewIdResponse(componentId));
                    _unnumberedComponentConnections.Remove(wrappedMessage.ComponentConnection);
                    _componentConnections.TryAdd(componentId, wrappedMessage.ComponentConnection);
                    var hcIdRequestMessage = (HeathCliffNewIdRequest)wrappedMessage.Message;
                    var connectToResponse = _manager.AddAndConnectTo(componentId, hcIdRequestMessage.ComponentType, wrappedMessage.ComponentConnection);
                    var connectToIp = _ipAddress;
                    var connectToPort = _port;
                    var connectToType = ComponentType.HeathCliff;
                    if (connectToResponse != -1)
                    {
                        var componentConnection = _manager.GetComponentConnection(connectToResponse);
                        connectToIp = componentConnection.IpAddress;
                        connectToPort = componentConnection.Port;
                        connectToType = hcIdRequestMessage.ComponentType;
                    }

                    var hcOrderConnect = new HeathCliffOrderConnect(connectToIp, connectToPort, connectToResponse, connectToType);
                    wrappedMessage.ComponentConnection.SendMessage(hcOrderConnect); // TODO: Send over MessageHub rather then directly? [1/3]
                }
            }
        }

        private int _messageCounter;
        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("{0} - {1} SourceComponentId:{2}", _messageCounter++, message, message.SourceComponent);
        }
    }
}
