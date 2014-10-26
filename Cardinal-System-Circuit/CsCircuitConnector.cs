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
        private readonly ConcurrentDictionary<int, CsCircuitComponentConnection> _componentConnections;
        private readonly List<CsCircuitComponentConnection> _unnumberedComponentConnections;
        private readonly ConcurrentQueue<MessageDto> _receivingQueue;
        private readonly Task _listenerTask;
        private readonly Task _distributingTask;

        private readonly Dictionary<int, IPEndPoint> _componentAddresses;
        private readonly Dictionary<long, List<int>> _componentsInterestInEntity;

        public CsCircuitConnector(IPAddress ipAddress, int port)
        {

            _componentAddresses = new Dictionary<int, IPEndPoint>();
            _componentsInterestInEntity = new Dictionary<long, List<int>>();

            _receivingQueue = new ConcurrentQueue<MessageDto>();
            _componentConnections = new ConcurrentDictionary<int, CsCircuitComponentConnection>();
            _ipAddress = ipAddress;
            _port = port;
            _unnumberedComponentConnections = new List<CsCircuitComponentConnection>();
            _tcpListener = new TcpListener(ipAddress, port);
            _listenerTask = new Task(DoListening);
            _distributingTask = new Task(DoDistributing);
        }


        private void DoDistributing()
        {
            while (!_receivingQueue.IsEmpty)
            {
                MessageDto messageDto;
                if (_receivingQueue.TryDequeue(out messageDto))
                {
                    // Until I create a way of adding component IDs after connecting this is useless

                    //int targetEntity = messageDto.TargetId;
                    //if (_componentsInterestInEntity.ContainsKey(targetEntity) && _componentsInterestInEntity[targetEntity].Count > 0)
                    //{
                    //    foreach (var componentInterestInEntity in _componentsInterestInEntity[targetEntity])
                    //    {
                    //        if (_componentAddresses.ContainsKey(componentInterestInEntity)
                    //            _componentConnections[componentInterestInEntity].GetSenderQueue.Enqueue(messageDto);
                    //    }
                    //}

                    // So for now send it to everything
                    foreach (var csCircuitComponentConnection in _componentConnections)
                    {
                        csCircuitComponentConnection.Value.GetSenderQueue.Enqueue(messageDto);
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

        public void RegisterComponentAddress(int componentId, IPEndPoint endpoint)
        {
            _componentAddresses.Add(componentId, endpoint);
        }

        public void RegisterComponentInterestInEntity(int componentId, int entityId)
        {
            if (!_componentsInterestInEntity.ContainsKey(componentId))
                _componentsInterestInEntity.Add(entityId, new List<int>());
            _componentsInterestInEntity[entityId].Add(componentId);
        }

    }
}