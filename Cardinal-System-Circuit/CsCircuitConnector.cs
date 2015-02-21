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
    public class CsCircuitConnector : Getter<MessageDto>
    {
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentDictionary<long, CsComponentConnection> _componentConnections;
        private readonly List<CsComponentConnection> _unnumberedComponentConnections;
        private readonly Task _listenerTask;

        private bool _doProcessing;

        public CsCircuitConnector(string address, int port)
        {
            _componentConnections = new ConcurrentDictionary<long, CsComponentConnection>();
            _unnumberedComponentConnections = new List<CsComponentConnection>();
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
            _doProcessing = true;
            _listenerTask.Start();
        }

        protected override void ExtraMessageRegisters()
        {
            MessageHubV2.Register<ConnectToHeathCliffRequest>(this, ConnectToHeathCliffRequest);
            MessageHubV2.Register<ConnectToHeathCliffResponse>(this, ConnectToHeathCliffResponse);
            MessageHubV2.Register<ComponentConnectionDisconnect>(this, DisconnectedComponent);
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
                MessageHubV2.Send(new ConnectToHeathCliffResponse(true));
                componentConnection.SendMessage(new HeathCliffOrderConnect());
            }
            catch (Exception)
            {
                MessageHubV2.Send(new ConnectToHeathCliffResponse(false));
            }


        }

        protected override void SpecificAction(MessageDto messageDto)
        {
            Console.WriteLine("Received message Family:{0} Type:{1} SourceId:{2} TargetId:{3}",
                messageDto.Family, messageDto.Type, messageDto.SourceId, messageDto.TargetId);
            switch (messageDto.Family)
            {
                case MessageFamily.PhysicalEntity:
                case MessageFamily.Component:
                case MessageFamily.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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