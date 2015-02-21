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
using Cardinal_System_Shared.Component;

namespace Cardinal_System_Circuit
{
    public class CsCircuitConnector
    {
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentDictionary<long, CsComponentConnection> _componentConnections = new ConcurrentDictionary<long, CsComponentConnection>();
        private readonly List<CsComponentConnection> _unnumberedComponentConnections = new List<CsComponentConnection>();
        private readonly Task _listenerTask;

        private bool _doProcessing;

        public CsCircuitConnector(string address, int port)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(address), port);
            _listenerTask = new Task(DoListening);
        }

        private void DisconnectedComponent(ComponentConnectionDisconnect disconnectedComponentConnection)
        {
            if (_unnumberedComponentConnections.Contains(disconnectedComponentConnection.CsComponentConnection))
            {
                _unnumberedComponentConnections.Remove(disconnectedComponentConnection.CsComponentConnection);
                return;
            }
            var connectionkey = _componentConnections.FirstOrDefault(x => x.Value == disconnectedComponentConnection.CsComponentConnection).Key;
            CsComponentConnection connection;
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
            MessageHubV2.Register<HeartbeatMessage>(this, HandleHeartbeatMessage);
        }

        private void HandleHeartbeatMessage(HeartbeatMessage heartbeatMessage)
        {
            var componentIdToSendTo = heartbeatMessage.ComponentId;
            CsComponentConnection componentConnection;
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
            var componentConnection = new CsComponentConnection(connectToHeathCliffRequest.Address, connectToHeathCliffRequest.Port);
            try
            {
                componentConnection.Start();
                _componentConnections.TryAdd(-1, componentConnection);
                MessageHubV2.Send(new ConnectToHeathCliffResponse(true));
                componentConnection.SendMessage(new HeathCliffOrderConnect());
                componentConnection.SendMessage(new HeathCliffOrderDisconnect());
                componentConnection.SendMessage(new HeathCliffNewIdRequest());
            }
            catch (Exception)
            {
                MessageHubV2.Send(new ConnectToHeathCliffResponse(false));
            }
        }

        public void Stop()
        {
            _doProcessing = false;
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (_doProcessing)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new CsComponentConnection(client);
                componentConnection.Start();
                _unnumberedComponentConnections.Add(componentConnection);
            }
        }

    }
}