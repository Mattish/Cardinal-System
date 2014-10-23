using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Circuit
{
    public class CsCircuitListener
    {
        private readonly int _port;
        private readonly ConcurrentQueue<MessageDto> _received;
        private Task _listener;

        public CsCircuitListener(int port, ConcurrentQueue<MessageDto> received)
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
                            _received.Enqueue(entityDto);
                        }
                    }
                }
            });
            _listener.Start();
        }
    }
}