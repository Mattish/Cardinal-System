using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared.Dtos;
using Cardinal_System_Shared.Dtos.Component;
using Newtonsoft.Json;

namespace Cardinal_System_HeathCliff
{
    public class CsHeathCliff : Getter<Message>, ICsNode
    {
        private readonly int _port;
        private readonly Stack<CsComponentRequestInfo> _circuitStack;
        private readonly Stack<CsComponentRequestInfo> _nodeStack;
        private readonly List<CsComponentConnection> connections;

        private TcpListener _tcpListener;
        private Task _listenerTask;
        private bool _doProcessing;
        private long _nextComponentId;

        public CsHeathCliff(int port)
        {
            connections = new List<CsComponentConnection>();
            _port = port;
            _nextComponentId = 1;
            _circuitStack = new Stack<CsComponentRequestInfo>();
            _nodeStack = new Stack<CsComponentRequestInfo>();
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (_doProcessing)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new CsComponentConnection(client);
                connections.Add(componentConnection);
            }
        }

        private void GotMessage(MessageDto dto, CsComponentConnection connection)
        {
            if (dto.Type == MessageType.InitialInfoRequest)
            {
                var requestInfo = JsonConvert.DeserializeObject<CsComponentRequestInfo>(dto.MessageObj);
                //TODO: Logic for balancing out components
                var componentInfo = new CsComponentRequest
                {
                    Id = _nextComponentId++,
                    NodeType = _circuitStack.Count == 0 ? CsNodeType.Circuit : CsNodeType.Server,
                    Circuits = _circuitStack.ToArray()
                };
                connection.SendMessage(new InitialInfoResponseMessage
                {
                    MessageObj = componentInfo
                }.ToDto());
                _nodeStack.Push(new CsComponentRequestInfo
                {
                    Id = componentInfo.Id,
                    IpAddress = requestInfo.IpAddress,
                    Port = requestInfo.Port
                });
            }
        }

        public void Stop()
        {
            _doProcessing = false;
        }

        public void Start()
        {
            _doProcessing = true;
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _listenerTask = Task.Factory.StartNew(DoListening);
        }

        protected override void SpecificAction(Message request)
        {
            Console.WriteLine(request);
        }
    }
}
