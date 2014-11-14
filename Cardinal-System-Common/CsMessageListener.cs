using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Common
{
    public class CsMessageListener : IAsyncRunnable
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _received;
        private Task _listener;

        public CsMessageListener(TcpClient client, ConcurrentQueue<MessageDto> received)
        {
            _received = received;
            _client = client;
        }

        public void Start()
        {
            _listener = new Task(() =>
            {
                _client.Client.ReceiveBufferSize = short.MaxValue;
                var stream = _client.GetStream();
                var serializer = new JsonSerializer
                {
                    CheckAdditionalContent = false
                };
                try
                {
                    var sr = new StreamReader(stream, Encoding.UTF8, false, 8096, true);
                    var jsr = new JsonTextReader(sr)
                    {
                        SupportMultipleContent = true
                    };

                    while (_client.Connected)
                    {
                        var dtoArray = serializer.Deserialize<MessageDtoArray>(jsr);
                        foreach (var entityDto in dtoArray.MessageDtos)
                        {
                            _received.Enqueue(entityDto);
                        }
                        if (!sr.EndOfStream)
                            jsr.Read();
                    }
                    Console.WriteLine("disconnect listener");
                }
                catch (IOException e)
                {
                    Console.WriteLine("Client listener error, probably disconnect {0}", e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Client listener error :( {0}", e.Message);
                }

            });
            _listener.Start();
        }

        public bool IsRunning()
        {
            return _listener.Status == TaskStatus.Running;
        }
    }
}