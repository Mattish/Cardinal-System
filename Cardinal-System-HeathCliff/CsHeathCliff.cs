using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
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

        private int _messageCounter;
        protected override void SpecificAction(Message request)
        {
            //Console.WriteLine("{0} - {1}", _messageCounter++, request);
        }
    }
}
