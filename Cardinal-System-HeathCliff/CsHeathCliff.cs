using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentDictionary<long, CsComponentConnection> _componentConnections = new ConcurrentDictionary<long, CsComponentConnection>();

        private readonly List<CsComponentConnection> _unnumberedComponentConnections = new List<CsComponentConnection>(); // TODO: Use a thread-safe collection?

        private TcpListener _tcpListener;
        private Task _listenerTask;
        private bool _doProcessing;
        private long _nextComponentId;

        private void DisconnectedComponent(ComponentConnectionDisconnect disconnectedComponentConnection)
        {
            lock (_unnumberedComponentConnections)
            {
                if (_unnumberedComponentConnections.Contains(disconnectedComponentConnection.CsComponentConnection))
                {
                    _unnumberedComponentConnections.Remove(disconnectedComponentConnection.CsComponentConnection);
                    return;
                }
            }
            var connectionkey = _componentConnections.FirstOrDefault(x => x.Value == disconnectedComponentConnection.CsComponentConnection).Key;
            CsComponentConnection connection;
            _componentConnections.TryRemove(connectionkey, out connection);
        }

        public CsHeathCliff(int port)
        {
            _port = port;
        }

        private void DoListening()
        {
            _tcpListener.Start();
            while (_doProcessing)
            {
                var client = _tcpListener.AcceptTcpClient();
                Console.WriteLine("Got connection!");
                var componentConnection = new CsComponentConnection(client); //TODO: When we receive a connection, will it always be unnumbered?
                lock (_unnumberedComponentConnections)
                {
                    _unnumberedComponentConnections.Add(componentConnection);
                }
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
                lock (_unnumberedComponentConnections)
                {
                    wrappedMessage.CsComponentConnection.SendMessage(new HeathCliffNewIdResponse(_nextComponentId++));
                    _unnumberedComponentConnections.Remove(wrappedMessage.CsComponentConnection);
                    _componentConnections.TryAdd(_nextComponentId, wrappedMessage.CsComponentConnection);
                }

            }
        }

        private int _messageCounter;
        protected override void SpecificAction(Message message)
        {
            Console.WriteLine("{0} - {1} SourceComponentId:{2}", _messageCounter++, message, message.SourceComponent);
        }
    }
}
