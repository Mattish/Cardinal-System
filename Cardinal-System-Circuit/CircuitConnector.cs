using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Circuit.InternalMessages;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;
using Cardinal_System_Shared.Dto;

namespace Cardinal_System_Circuit
{
    public class CircuitConnector
    {
        private readonly string _address;
        private readonly int _port;
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentDictionary<long, ComponentConnection> _componentConnections = new ConcurrentDictionary<long, ComponentConnection>();
        private readonly List<ComponentConnection> _unnumberedComponentConnections = new List<ComponentConnection>();
        private readonly Task _listenerTask;

        private bool _doProcessing;

        public CircuitConnector(string address, int port)
        {
            _address = address;
            _port = port;
            _tcpListener = new TcpListener(IPAddress.Parse(address), port);
            _listenerTask = new Task(DoListening);
        }

        private void DisconnectedComponent(ComponentConnectionDisconnect disconnectedComponentConnection)
        {
            if (_unnumberedComponentConnections.Contains(disconnectedComponentConnection.ComponentConnection))
            {
                _unnumberedComponentConnections.Remove(disconnectedComponentConnection.ComponentConnection);
                return;
            }
            var connectionkey = _componentConnections.FirstOrDefault(x => x.Value == disconnectedComponentConnection.ComponentConnection).Key;
            ComponentConnection connection;
            _componentConnections.TryRemove(connectionkey, out connection);
        }

        public void Start()
        {
            ExtraMessageRegisters();
            _doProcessing = true;
            _listenerTask.Start();
        }

        private void ExtraMessageRegisters()
        {
            MessageHubV2.Register<ConnectToHeathCliffRequest>(this, ConnectToHeathCliffRequest);
            MessageHubV2.Register<ConnectToHeathCliffResponse>(this, ConnectToHeathCliffResponse);
            MessageHubV2.Register<ComponentConnectionDisconnect>(this, DisconnectedComponent);
            MessageHubV2.Register<DisconnectFromHeathCliff>(this, DisconnectFromHeathCliff);
            MessageHubV2.Register<HeartbeatMessage>(this, HandleHeartbeatMessage);

            MessageHubV2.Register<MessageDto>(this, HandleDtoNotForUs);
        }

        private void HandleDtoNotForUs(MessageDto messageDto)
        {
            if (_componentConnections.ContainsKey(messageDto.TargetComponent))
            {
                _componentConnections[messageDto.TargetComponent].SendMessageDto(messageDto);
                Console.WriteLine("Forwarding {0} to {1}", messageDto.Type, messageDto.TargetComponent);
            }
            else
            {
                throw new Exception("Received message not for us, but don't have connection to intended target");
            }
        }

        private void HandleHeartbeatMessage(HeartbeatMessage heartbeatMessage)
        {
            var componentIdToSendTo = heartbeatMessage.ComponentId;
            ComponentConnection componentConnection;
            if (_componentConnections.TryGetValue(componentIdToSendTo, out componentConnection))
            {
                componentConnection.SendMessage(new Heartbeat());
            }
        }

        private void ConnectToHeathCliffResponse(ConnectToHeathCliffResponse connectToHeathCliffResponse)
        {
            if (connectToHeathCliffResponse.Success)
                Console.WriteLine("Connected to HC");
            else
                Console.WriteLine("Failed Connecting to HC");
        }

        private void ConnectToHeathCliffRequest(ConnectToHeathCliffRequest connectToHeathCliffRequest)
        {
            var componentConnection = new ComponentConnection(connectToHeathCliffRequest.Address, connectToHeathCliffRequest.Port);
            try
            {
                componentConnection.Start();
                _componentConnections.TryAdd(-1, componentConnection);
                MessageHubV2.Send(new ConnectToHeathCliffResponse(true));
                componentConnection.SendMessage(new HeathCliffNewIdRequest(ComponentSettings.ComponentType, _address, _port));
            }
            catch (Exception)
            {
                MessageHubV2.Send(new ConnectToHeathCliffResponse(false));
            }
        }

        private void DisconnectFromHeathCliff(DisconnectFromHeathCliff disconnectFromHeathCliff)
        {
            ComponentConnection componentConnection;
            if (_componentConnections.TryGetValue(-1, out componentConnection))
            {
                componentConnection.Stop();
                _componentConnections.TryRemove(-1, out componentConnection);
            }

            if (!_componentConnections.ContainsKey(disconnectFromHeathCliff.ComponentId))
            {
                componentConnection = new ComponentConnection(disconnectFromHeathCliff.IpAddress, disconnectFromHeathCliff.Port);
                try
                {
                    componentConnection.Start();
                    _componentConnections.TryAdd(disconnectFromHeathCliff.ComponentId, componentConnection);
                    ComponentSettings.DefaultCircuit = disconnectFromHeathCliff.ComponentId;

                    //TODO: What do after connecting to another circuit? [1/3]
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Couldn't connect to {0}:{1}", disconnectFromHeathCliff.IpAddress, disconnectFromHeathCliff.Port));
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
                if (_componentConnections.TryGetValue(ComponentSettings.DefaultCircuit, out componentConnection))
                {
                    componentConnection.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No ComponentConnection by the DefaultCircuit:{0}", ComponentSettings.DefaultCircuit);
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
                _unnumberedComponentConnections.Add(componentConnection);
            }
        }

    }
}