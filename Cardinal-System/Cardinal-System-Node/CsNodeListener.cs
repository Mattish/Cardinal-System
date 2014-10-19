using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public class CsNodeListener
    {
        private readonly int _port;
        private readonly ConcurrentQueue<EntityChangeWrapper> _received;
        private Task _listener;

        public CsNodeListener(int port, ConcurrentQueue<EntityChangeWrapper> received)
        {
            _port = port;
            _received = received;
        }

        public void Start()
        {
            _listener = new Task(async () =>
            {
                using (var udpListener = new UdpClient(_port))
                {
                    while (true)
                    {
                        var udpResult = await udpListener.ReceiveAsync();
                        byte[] bytesArray = udpResult.Buffer;
                        string json = Encoding.UTF8.GetString(bytesArray);
                        var entityDtoArray = JsonConvert.DeserializeObject<EntityChangeDtoArray>(json);
                        foreach (var entityDto in entityDtoArray.EntityChangeDtos)
                        {
                            _received.Enqueue(TranslateFromDto(entityDto));
                        }
                    }
                }
            });
            _listener.Start();
        }

        private EntityChangeWrapper TranslateFromDto(EntityChangeDto changeDto)
        {
            var wrapper = new EntityChangeWrapper { Type = changeDto.Type };
            switch (changeDto.Type)
            {
                case EntityChangeType.PhysicalPosition:
                    wrapper.EntityChange = JsonConvert.DeserializeObject<PhysicalMovementEntityChange>(changeDto.EntityChange);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return wrapper;
        }
    }
}