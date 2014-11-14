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
        private readonly Task _listenerTask;
        private readonly Task _distributingTask;

        private readonly Dictionary<long, List<long>> _componentsInterestInEntity;
        private bool doProcessing;

        public CsCircuitConnector(IPAddress ipAddress, int port)
        {
            _componentsInterestInEntity = new Dictionary<long, List<long>>();

            _componentConnections = new ConcurrentDictionary<long, CsCircuitComponentConnection>();
            _ipAddress = ipAddress;
            _port = port;
            _unnumberedComponentConnections = new List<CsCircuitComponentConnection>();
            _tcpListener = new TcpListener(ipAddress, port);
            _listenerTask = new Task(DoListening);
            _distributingTask = new Task(ProcessMessagingLoop);
        }


        private void ProcessMessagingLoop()
        {
            while (doProcessing)
            {
                for (int index = 0; index < _unnumberedComponentConnections.Count; index++)
                {
                    var unnumberedComponentConnection = _unnumberedComponentConnections[index];
                    if (!unnumberedComponentConnection.ReceiverQueue.IsEmpty)
                    {
                        MessageDto messageDto;
                        if (unnumberedComponentConnection.ReceiverQueue.TryDequeue(out messageDto))
                        {
                            if (messageDto.Type == MessageType.RegisterWithCircuit)
                            {
                                var registerWithCircuitMessage = messageDto.TranslateFromDto() as RegisterWithCircuitMessage;
                                var resultOfNewComponentRegister = _componentConnections.TryAdd(registerWithCircuitMessage.SourceId, unnumberedComponentConnection);
                                if (resultOfNewComponentRegister)
                                {
                                    _unnumberedComponentConnections.Remove(unnumberedComponentConnection);
                                    index--;
                                }
                                Console.WriteLine(
                                    !resultOfNewComponentRegister
                                        ? "Was unable to tryAdd component connection - SourceId:{0}"
                                        : "Added component connection - SourceId:{0}", messageDto.SourceId);
                            }
                        }
                    }
                }
                foreach (var componentConnection in _componentConnections)
                {
                    if (!componentConnection.Value.ReceiverQueue.IsEmpty)
                    {
                        MessageDto messageDto;
                        if (componentConnection.Value.ReceiverQueue.TryDequeue(out messageDto))
                        {
                            Console.WriteLine("Received message Family:{0} Type:{1} SourceId:{2} TargetId:{3}",
                            messageDto.Family, messageDto.Type, messageDto.SourceId, messageDto.TargetId);
                            switch (messageDto.Family)
                            {
                                case MessageFamily.PhysicalEntity:
                                    // So for now send it to everything
                                    foreach (var csCircuitComponentConnection in _componentConnections)
                                    {
                                        csCircuitComponentConnection.Value.SenderQueue.Enqueue(messageDto);
                                    }
                                    break;
                                case MessageFamily.Component:
                                    switch (messageDto.Type)
                                    {
                                        case MessageType.RegisterEntityInterest:
                                            var registerEntityInterestMessage = messageDto.TranslateFromDto() as RegisterEntityInterestMessage;
                                            RegisterComponentInterestInEntity(registerEntityInterestMessage.SourceId, registerEntityInterestMessage.TargetId);
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
        }

        public void Start()
        {
            doProcessing = true;
            _listenerTask.Start();
            _distributingTask.Start();
        }

        public void Stop()
        {
            doProcessing = false;
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (doProcessing)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new CsCircuitComponentConnection(client);
                componentConnection.Start();
                _unnumberedComponentConnections.Add(componentConnection);
            }
        }

        public void RegisterComponentInterestInEntity(long componentId, long entityId)
        {
            if (!_componentsInterestInEntity.ContainsKey(componentId))
                _componentsInterestInEntity.Add(entityId, new List<long>());
            _componentsInterestInEntity[entityId].Add(componentId);
        }

    }
}