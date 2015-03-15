using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Common.InternalMessages;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_Common.MessageNetworking
{
    public abstract class Node : Getter<Message>
    {
        private readonly string _address;
        private readonly int _port;
        private readonly TcpListener _tcpListener;
        private readonly List<ComponentConnection> _unnumberedComponentConnections = new List<ComponentConnection>();
        private readonly Task _listenerTask;
        private Heartbeater _heartbeater;
        private bool _doProcessing;

        protected readonly ConcurrentDictionary<long, ComponentConnection> ComponentConnections =
            new ConcurrentDictionary<long, ComponentConnection>();

        protected Node(string address, int port)
        {
            _address = address;
            _port = port;
            _tcpListener = new TcpListener(IPAddress.Parse(address), port);
            _listenerTask = new Task(DoListening);
        }

        public void Start()
        {
            _doProcessing = true;
            _listenerTask.Start();
            MessageHubV2.Send(new ConnectToHeathCliffRequest(ConfigurationManager.AppSettings["HCAddress"],
                int.Parse(ConfigurationManager.AppSettings["HCPort"])));
        }

        protected override void ExtraMessageRegisters()
        {
            MessageHubV2.Register<ConnectToHeathCliffRequest>(this, ConnectToHeathCliffRequest);
            MessageHubV2.Register<ConnectToHeathCliffResponse>(this, ConnectToHeathCliffResponse);
            MessageHubV2.Register<ComponentConnectionDisconnect>(this, DisconnectedComponent);
            MessageHubV2.Register<DisconnectFromHeathCliff>(this, DisconnectFromHeathCliff);
            MessageHubV2.Register<HeartbeatMessage>(this, HandleHeartbeatMessage);
            MessageHubV2.Register<WrappedMessage>(this, HandleWrappedMessage);
            ComponentSpecificMessageRegisters();
        }

        protected abstract void ComponentSpecificMessageRegisters();

        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("{0} SourceComponentId:{1} TargetComponentId:{2}", message, message.SourceComponent,
                message.TargetComponent);
            if (message.Type == MessageType.HeathCliffNewIdResponse)
            {
                var newComponentId = ((HeathCliffNewIdResponse) message).NewId;
                ComponentSettings.ComponentId = newComponentId;
                Console.WriteLine("We are now ComponentId:{0}", newComponentId);
                ComponentConnection componentConnection;
                if (ComponentConnections.TryGetValue(-1, out componentConnection))
                {
                    if (_heartbeater == null)
                    {
                        try
                        {
                            var secondsPerHeartbeat = ConfigurationManager.AppSettings["secondsPerHeartbeat"];
                            var secondsPerHeartbeatint = int.Parse(secondsPerHeartbeat);
                            _heartbeater = new Heartbeater(componentConnection, secondsPerHeartbeatint);
                            _heartbeater.Start();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unable to start up Heartbeater for Id:{0}: Type:{1} - {2}",
                                ComponentSettings.ComponentId, ComponentSettings.ComponentType, e);
                        }
                    }
                }
            }
            else if (message.Type == MessageType.HeathCliffOrderConnect)
            {
                var hcOrderConnectMessage = (HeathCliffOrderConnect) message;
                Console.WriteLine("HeathCliffOrderConnect - IpAddress:{0} Port:{1} Id:{2} Type:{3}",
                    hcOrderConnectMessage.IpAddress, hcOrderConnectMessage.Port, hcOrderConnectMessage.ComponentId,
                    hcOrderConnectMessage.ComponentType);

                if (hcOrderConnectMessage.ComponentId != -1)
                {
                    MessageHubV2.Send(new DisconnectFromHeathCliff(hcOrderConnectMessage.IpAddress,
                        hcOrderConnectMessage.Port, hcOrderConnectMessage.ComponentId));
                }
            }
        }

        private void HandleWrappedMessage(WrappedMessage wrappedMessage)
        {
            if (wrappedMessage.Message.Type == MessageType.ComponentInformationBroadcast)
            {
                _unnumberedComponentConnections.Remove(wrappedMessage.ComponentConnection);
                var componentInformationBroadcastMessage = (ComponentInformationBroadcast) wrappedMessage.Message;
                ComponentConnections.TryAdd(componentInformationBroadcastMessage.ComponentId,
                    wrappedMessage.ComponentConnection);
                Console.WriteLine("ComponentInformationBroadcast - Node:{0} Type:{1}",
                    componentInformationBroadcastMessage.ComponentId, componentInformationBroadcastMessage.ComponentType);
            }
            else
            {
                SpecificAction(wrappedMessage.Message);
            }
        }

        private void DisconnectedComponent(ComponentConnectionDisconnect disconnectedComponentConnection)
        {
            if (_unnumberedComponentConnections.Contains(disconnectedComponentConnection.ComponentConnection))
            {
                _unnumberedComponentConnections.Remove(disconnectedComponentConnection.ComponentConnection);
                Console.WriteLine("Disconnected from unnumbered node");
                return;
            }
            var pair =
                ComponentConnections.FirstOrDefault(x => x.Value == disconnectedComponentConnection.ComponentConnection);
            if (pair.Value != null)
            {
                ComponentConnection connection;
                ComponentConnections.TryRemove(pair.Key, out connection);
                Console.WriteLine("Disconnected from node:{0}", pair.Key);
            }
            else
            {
                Console.WriteLine("Disconnected from node called, but no node in collection!");
            }
        }

        private void HandleHeartbeatMessage(HeartbeatMessage heartbeatMessage)
        {
            var componentIdToSendTo = heartbeatMessage.ComponentId;
            ComponentConnection componentConnection;
            if (ComponentConnections.TryGetValue(componentIdToSendTo, out componentConnection))
            {
                componentConnection.SendMessage(new Heartbeat());
            }
        }

        private void ConnectToHeathCliffResponse(ConnectToHeathCliffResponse connectToHeathCliffResponse)
        {
            if (connectToHeathCliffResponse.Success)
            {
                Console.WriteLine("Connected to HC");
            }
            else
                Console.WriteLine("Failed Connecting to HC");
        }

        private void ConnectToHeathCliffRequest(ConnectToHeathCliffRequest connectToHeathCliffRequest)
        {
            var componentConnection = new ComponentConnection(connectToHeathCliffRequest.Address,
                connectToHeathCliffRequest.Port);
            try
            {
                componentConnection.Start();
                ComponentConnections.TryAdd(-1, componentConnection);
                MessageHubV2.Send(new ConnectToHeathCliffResponse(true));
                componentConnection.SendMessage(new HeathCliffNewIdRequest(ComponentSettings.ComponentType, _address,
                    _port));
            }
            catch (Exception)
            {
                MessageHubV2.Send(new ConnectToHeathCliffResponse(false));
            }
        }

        private void DisconnectFromHeathCliff(DisconnectFromHeathCliff disconnectFromHeathCliff)
        {
            ComponentConnection componentConnection;
            if (ComponentConnections.TryGetValue(-1, out componentConnection))
            {
                componentConnection.Stop();
            }

            ConnectToNewComponent(disconnectFromHeathCliff.ComponentId, disconnectFromHeathCliff.IpAddress,
                disconnectFromHeathCliff.Port);
        }

        protected virtual void ConnectToNewComponent(long componentId, string ipAddress, int port)
        {
            ComponentConnection componentConnection;
            if (!ComponentConnections.ContainsKey(componentId))
            {
                componentConnection = new ComponentConnection(ipAddress, port);
                try
                {
                    Console.WriteLine("DisconnectFromHeathCliff, Connecting to {0} - {1}", ipAddress, port);
                    componentConnection.Start();
                    Console.WriteLine("DisconnectFromHeathCliff, Connected!");
                    ComponentConnections.TryAdd(componentId, componentConnection);
                    if (ComponentSettings.ComponentType == ComponentType.Server)
                        // TODO: Set this for other component types?
                        ComponentSettings.DefaultCircuit = componentId;
                    _heartbeater.ReassignComponentConnection(componentConnection);
                    var messageToGoOut = new ComponentInformationBroadcast(ComponentSettings.ComponentId,
                        ComponentSettings.ComponentType);
                    componentConnection.SendMessage(messageToGoOut);
                    //TODO: What do after connecting to another circuit? [1/3]
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Couldn't connect to {0}:{1}", ipAddress, port));
                    //TODO: If can't connect, do something nicer [1/3]
                }
            }
        }

        public void Stop()
        {
            _doProcessing = false;
        }

        public void SendMessage(Message message)
        {
            if (ComponentSettings.DefaultCircuit != 0)
            {
                ComponentConnection componentConnection;
                if (ComponentConnections.TryGetValue(ComponentSettings.DefaultCircuit, out componentConnection))
                {
                    componentConnection.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No ComponentConnection by the DefaultCircuit:{0}",
                        ComponentSettings.DefaultCircuit);
                }
            }
            else
            {
                Console.WriteLine("Dont have a DefaultCircuit!");
            }
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (_doProcessing)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new ComponentConnection(client);
                componentConnection.Start();
                Console.WriteLine("Connection started!");
                _unnumberedComponentConnections.Add(componentConnection);
            }
        }
    }
}