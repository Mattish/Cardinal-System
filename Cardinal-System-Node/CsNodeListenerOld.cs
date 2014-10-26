using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public class CsNodeListenerOld
    {
        private readonly int _port;
        private readonly ConcurrentQueue<MessageWrapper> _received;
        private Task _listener;

        public CsNodeListenerOld(int port, ConcurrentQueue<MessageWrapper> received)
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
                    udpListener.Client.ReceiveBufferSize = short.MaxValue;
                    while (true)
                    {
                        var udpResult = await udpListener.ReceiveAsync();
                        byte[] bytesArray = udpResult.Buffer;
                        string json = Encoding.UTF8.GetString(bytesArray);
                        var entityDtoArray = JsonConvert.DeserializeObject<MessageDtoArray>(json);
                        foreach (var entityDto in entityDtoArray.MessageDtos)
                        {
                            _received.Enqueue(TranslateFromDto(entityDto));
                        }
                    }
                }
            });
            _listener.Start();
        }

        private MessageWrapper TranslateFromDto(MessageDto changeDto)
        {
            var wrapper = new MessageWrapper { Type = changeDto.Type };
            switch (changeDto.Type)
            {
                case MessageType.PhysicalEntityPosition:
                    wrapper.Message = JsonConvert.DeserializeObject<PhysicalMovementMessage>(changeDto.Message);
                    break;
                case MessageType.PhysicalEntityCreate:
                    wrapper.Message = JsonConvert.DeserializeObject<PhysicalCreateMessage>(changeDto.Message);
                    break;
            }
            return wrapper;
        }
    }
}