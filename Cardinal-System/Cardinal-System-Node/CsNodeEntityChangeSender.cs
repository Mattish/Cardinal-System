using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public class CsNodeEntityChangeSender
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
            var endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _udpSender.Send(buffer, buffer.Length, endPoint);
        }

        public void SendMany<T>(List<T> objectsToSend, string host, int port) where T : EntityChange
        {
            if (_udpSender == null)
                _udpSender = new UdpClient();
            var array = new EntityChangeDtoArray();
            var listOfDtos = new List<EntityChangeDto>(16);
            int startIndex = 0;
            int addIndex = 0;
            while (startIndex < objectsToSend.Count())
            {
                while (startIndex + addIndex < objectsToSend.Count() && addIndex < 16)
                {
                    listOfDtos.Add(new EntityChangeDto
                    {
                        EntityChange = JsonConvert.SerializeObject(objectsToSend[startIndex + addIndex]),
                        Type = objectsToSend[startIndex + addIndex].GetEntityChangeType()
                    });
                    addIndex++;
                }

                array.EntityChangeDtos = listOfDtos;
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