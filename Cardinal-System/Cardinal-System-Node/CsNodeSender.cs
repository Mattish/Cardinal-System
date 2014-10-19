using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public class CsNodeSender
    {
        private UdpClient _udpSender;

        public void Send<T>(T objectToSend, string host, int port) where T : EntityChange
        {
            if (_udpSender == null)
                _udpSender = new UdpClient();
            var array = new EntityChangeDtoArray
            {
                EntityChangeDtos = new[]
                {
                    new EntityChangeDto
                    {
                        EntityChange = JsonConvert.SerializeObject(objectToSend),
                        Type = objectToSend.GetEntityChangeType()
                    }
                }
            };
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(array));
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            _udpSender.Send(buffer, buffer.Length, endPoint);
        }
    }

    public class CsEntityChangeSender
    {
        private readonly IPEndPoint _ipEndPoint;
        private readonly List<ChangeableState> changeableStates;
        private readonly CsNodeSender _sender;

        public CsEntityChangeSender(IPEndPoint ipEndPoint)
        {
            _ipEndPoint = ipEndPoint;
            changeableStates = new List<ChangeableState>();
            _sender = new CsNodeSender();
        }

        public void RegisterChanged(ChangeableState changeableState)
        {
            changeableStates.Add(changeableState);
        }

        public void SendChanges()
        {
            foreach (ChangeableState changeableState in changeableStates)
            {

            }
        }
    }

    public class CsEntityChangeListener{

        public CsEntityChangeListener()
        {
        }
    }
}