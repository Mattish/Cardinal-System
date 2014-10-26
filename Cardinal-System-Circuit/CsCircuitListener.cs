using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Circuit
{
    public class CsCircuitListener
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _received;
        private Task _listener;

        public CsCircuitListener(TcpClient client, ConcurrentQueue<MessageDto> received)
        {
            _received = received;
            _client = client;
        }

        public void Start()
        {
            int totalReceived = 0;
            _listener = new Task(() =>
            {
                using (_client)
                {
                    _client.Client.ReceiveBufferSize = short.MaxValue;
                    var stream = _client.GetStream();
                    var serializer = new JsonSerializer
                    {
                        CheckAdditionalContent = false
                    };

                    using (var sr = new StreamReader(stream, Encoding.UTF8, false))
                    using (var jsr = new JsonTextReader(sr)
                    {
                        SupportMultipleContent = true
                    })
                    {
                        while (_client.Connected)
                        {
                            var dtoArray = serializer.Deserialize<MessageDtoArray>(jsr);
                            totalReceived += dtoArray.MessageDtos.Count;
                            foreach (var entityDto in dtoArray.MessageDtos)
                            {
                                _received.Enqueue(entityDto);
                            }
                            Console.WriteLine("receivedTotal:{0}", _received.Count);
                            jsr.Read();
                        }
                    }
                }
            });
            _listener.Start();
        }
    }
}