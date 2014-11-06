using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Shared;

namespace Cardinal_System_Circuit
{
    public class CsCircuitConnector
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentDictionary<long, CsCircuitComponentConnection> _componentConnections;
        private readonly List<CsCircuitComponentConnection> _unnumberedComponentConnections;
        private readonly ConcurrentQueue<MessageDto> _receivingQueue;
        private readonly Task _listenerTask;
        private readonly Task _distributingTask;

        private readonly Dictionary<long, IPEndPoint> _componentAddresses;
        private readonly Dictionary<long, List<long>> _componentsInterestInEntity;

        public CsCircuitConnector(IPAddress ipAddress, int port)
        {
            _componentAddresses = new Dictionary<long, IPEndPoint>();
            _componentsInterestInEntity = new Dictionary<long, List<long>>();

            _receivingQueue = new ConcurrentQueue<MessageDto>();
            _componentConnections = new ConcurrentDictionary<long, CsCircuitComponentConnection>();
            _ipAddress = ipAddress;
            _port = port;
            _unnumberedComponentConnections = new List<CsCircuitComponentConnection>();
            _tcpListener = new TcpListener(ipAddress, port);
            _listenerTask = new Task(DoListening);
            _distributingTask = new Task(DoDistributing);
        }


        private void DoDistributing()
        {
            while (true)
            {
                while (!_receivingQueue.IsEmpty)
                {
                    MessageDto messageDto;
                    if (_receivingQueue.TryDequeue(out messageDto))
                    {
                        switch (messageDto.Family)
                        {
                            case MessageFamily.PhysicalEntity:
                                // So for now send it to everything
                                foreach (var csCircuitComponentConnection in _componentConnections)
                                {
                                    csCircuitComponentConnection.Value.GetSenderQueue.Enqueue(messageDto);
                                }
                                break;
                            case MessageFamily.Component:
                                switch (messageDto.Type)
                                {
                                    case MessageType.RegisterEntityInterest:
                                        var componentMessage =
                                            messageDto.TranslateFromDto() as RegisterEntityInterestMessage;
                                        Console.WriteLine(
                                            "Received message from componentId:{0} registering interest for targetId:{1}",
                                            componentMessage.SourceId, componentMessage.TargetId);
                                        if (componentMessage != null)
                                            RegisterComponentInterestInEntity(componentMessage.SourceId,
                                                componentMessage.TargetId);
                                        break;
                                }
                                break;
                            case MessageFamily.Unknown:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        public void Start()
        {
            _listenerTask.Start();
            _distributingTask.Start();
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (true)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new CsCircuitComponentConnection(client, _receivingQueue);
                componentConnection.Start();
                _unnumberedComponentConnections.Add(componentConnection);
            }
        }

        public void RegisterComponentAddress(long componentId, IPEndPoint endpoint)
        {
            _componentAddresses.Add(componentId, endpoint);
        }

        public void RegisterComponentInterestInEntity(long componentId, long entityId)
        {
            if (!_componentsInterestInEntity.ContainsKey(componentId))
                _componentsInterestInEntity.Add(entityId, new List<long>());
            _componentsInterestInEntity[entityId].Add(componentId);
        }

    }
}