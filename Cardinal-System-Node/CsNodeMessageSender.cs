using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public class CsNodeMessageSender
    {
        private UdpClient _udpSender;

        public void Send<T>(T objectToSend, string host, int port) where T : Message
        {
            if (_udpSender == null)
                _udpSender = new UdpClient();
            var array = new MessageDtoArray
            {
                MessageDtos = new[]
                {
                    new MessageDto
                    {
                        SourceId = objectToSend.SourceId,
                        TargetId = objectToSend.TargetId,
                        Message = JsonConvert.SerializeObject(objectToSend),
                        Type = objectToSend.GetMessageType()
                    }
                }
            };
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(array));
            var endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _udpSender.Send(buffer, buffer.Length, endPoint);
        }

        public void SendMany<T>(List<T> objectsToSend, string host, int port) where T : Message
        {
            if (_udpSender == null)
                _udpSender = new UdpClient();
            var array = new MessageDtoArray();
            var listOfDtos = new List<MessageDto>(16);
            int startIndex = 0;
            int addIndex = 0;
            while (startIndex < objectsToSend.Count())
            {
                while (startIndex + addIndex < objectsToSend.Count() && addIndex < 16)
                {
                    listOfDtos.Add(new MessageDto
                    {
                        SourceId = objectsToSend[startIndex + addIndex].SourceId,
                        TargetId = objectsToSend[startIndex + addIndex].TargetId,
                        Message = JsonConvert.SerializeObject(objectsToSend[startIndex + addIndex]),
                        Type = objectsToSend[startIndex + addIndex].GetMessageType()
                    });
                    addIndex++;
                }

                array.MessageDtos = listOfDtos;
                string json = JsonConvert.SerializeObject(array);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                var endPoint = new IPEndPoint(IPAddress.Parse(host), port);
                _udpSender.Send(buffer, buffer.Length, endPoint);
                startIndex += 16;
                addIndex = 0;
                listOfDtos.Clear();
            }
        }
    }
}