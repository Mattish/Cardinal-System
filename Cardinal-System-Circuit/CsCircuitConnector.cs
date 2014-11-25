using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Shared.Dtos;
using Cardinal_System_Shared.Dtos.Component;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Circuit
{
    public class CsCircuitConnector
    {
        private readonly TcpListener _tcpListener;
        private readonly ConcurrentDictionary<long, CsComponentConnection> _componentConnections;
        private readonly List<CsComponentConnection> _unnumberedComponentConnections;
        private readonly Task _listenerTask;

        private readonly Dictionary<EntityId, List<long>> _componentsInterestInEntity;
        private bool _doProcessing;

        public CsCircuitConnector(IPAddress ipAddress, int port)
        {
            _componentsInterestInEntity = new Dictionary<EntityId, List<long>>();

            _componentConnections = new ConcurrentDictionary<long, CsComponentConnection>();
            _unnumberedComponentConnections = new List<CsComponentConnection>();
            _tcpListener = new TcpListener(ipAddress, port);
            _listenerTask = new Task(DoListening);
        }

        private void DisconnectedComponent(CsComponentConnection disconnectedComponentConnection)
        {
            if (_unnumberedComponentConnections.Contains(disconnectedComponentConnection))
            {
                _unnumberedComponentConnections.Remove(disconnectedComponentConnection);
                return;
            }
            var connectionkey = _componentConnections.FirstOrDefault(x => x.Value == disconnectedComponentConnection).Key;
            CsComponentConnection connection;
            _componentConnections.TryRemove(connectionkey, out connection);
        }

        private void GotMessage(MessageDto messageDto, CsComponentConnection sender)
        {
            Console.WriteLine("Received message Family:{0} Type:{1} SourceId:{2} TargetId:{3}",
                messageDto.Family, messageDto.Type, messageDto.SourceId, messageDto.TargetId);
            switch (messageDto.Family)
            {
                case MessageFamily.PhysicalEntity:
                    // So for now send it to everything
                    foreach (var csCircuitComponentConnection in _componentConnections)
                    {
                        csCircuitComponentConnection.Value.SendMessage(messageDto);
                    }
                    break;
                case MessageFamily.Component:
                    switch (messageDto.Type)
                    {
                        case MessageType.RegisterEntityInterest:
                            var registerEntityInterestMessage = messageDto.TranslateFromDto() as RegisterEntityInterestMessage;
                            RegisterComponentInterestInEntity(registerEntityInterestMessage.SourceId.ComponentId, registerEntityInterestMessage.TargetId);
                            break;
                        case MessageType.RegisterWithCircuit:
                            var registerWithCircuitMessage = messageDto.TranslateFromDto() as RegisterWithCircuitMessage;
                            var resultOfNewComponentRegister = _componentConnections.TryAdd(registerWithCircuitMessage.SourceId.ComponentId, sender);
                            if (resultOfNewComponentRegister)
                            {
                                _unnumberedComponentConnections.Remove(sender);
                            }
                            Console.WriteLine(
                                !resultOfNewComponentRegister
                                    ? "Was unable to tryAdd component connection - SourceId:{0}"
                                    : "Added component connection - SourceId:{0}", messageDto.SourceId);
                            break;
                    }
                    break;
                case MessageFamily.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void Start()
        {
            _doProcessing = true;
            _listenerTask.Start();
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
                var componentConnection = new CsComponentConnection(client, GotMessage, DisconnectedComponent);
                componentConnection.Start();
                _unnumberedComponentConnections.Add(componentConnection);
            }
        }

        public void RegisterComponentInterestInEntity(long componentId, EntityId entityId)
        {
            if (!_componentsInterestInEntity.ContainsKey(entityId))
                _componentsInterestInEntity.Add(entityId, new List<long>());
            _componentsInterestInEntity[entityId].Add(componentId);
        }

    }
}