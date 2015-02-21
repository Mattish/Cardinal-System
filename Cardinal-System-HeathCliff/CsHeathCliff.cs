using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_HeathCliff
{
    public class CsHeathCliff : Getter<Message>, ICsNode
    {
        private readonly int _port;
        private readonly List<CsComponentConnection> connections;

        private TcpListener _tcpListener;
        private Task _listenerTask;
        private bool _doProcessing;
        private long _nextComponentId;

        public CsHeathCliff(int port)
        {
            connections = new List<CsComponentConnection>();
            _port = port;
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
            _nextComponentId = 1000;
            _doProcessing = true;
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _listenerTask = Task.Factory.StartNew(DoListening);
        }

        protected override void ExtraMessageRegisters()
        {
            MessageHubV2.Register<WrappedMessage>(this, HandleWrappedMessage);
        }

        private void HandleWrappedMessage(WrappedMessage wrappedMessage)
        {
            Console.WriteLine("{0} - {1} SourceComponentId:{2}", _messageCounter++, wrappedMessage.Message, wrappedMessage.Message.SourceComponent);
            if (wrappedMessage.Message.Type == MessageType.HeathCliffNewIdRequest)
            {
                wrappedMessage.CsComponentConnection.SendMessage(new HeathCliffNewIdResponse(_nextComponentId++));
            }
        }

        private int _messageCounter;
        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("{0} - {1} SourceComponentId:{2}", _messageCounter++, message, message.SourceComponent);
        }
    }
}
