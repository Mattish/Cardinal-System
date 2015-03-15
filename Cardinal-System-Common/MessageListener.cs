using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Dto;
using Newtonsoft.Json;

namespace Cardinal_System_Common
{
    public class MessageListener : IAsyncRunnable
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _received;
        private readonly Action _disconnectAction;
        private readonly ManualResetEventSlim _manualResetEventSlimReceiving;
        private Task _listener;
        private int _receiveTotal;

        public MessageListener(TcpClient client, ConcurrentQueue<MessageDto> received, Action disconnectAction,
            ManualResetEventSlim manualResetEventSlimReceiving)
        {
            _received = received;
            _disconnectAction = disconnectAction;
            _manualResetEventSlimReceiving = manualResetEventSlimReceiving;
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
                    CheckAdditionalContent = false,
                    Converters = {new MessageDtoConverter()}
                };
                try
                {
                    var sr = new StreamReader(stream, Encoding.UTF8, false, 2 ^ 16, true);
                    var jsr = new JsonTextReader(sr)
                    {
                        SupportMultipleContent = true,
                    };

                    while (_client.Connected)
                    {
                        var dtoArray = serializer.Deserialize<MessageDtoArray>(jsr);
                        foreach (var entityDto in dtoArray.Dtos)
                        {
                            _receiveTotal++;
                            _received.Enqueue(entityDto);
                            _manualResetEventSlimReceiving.Set();
                        }
                        //Console.WriteLine("receivedTotal:{0}", _receiveTotal);
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
                _disconnectAction();
            });
            _listener.Start();
        }

        public bool IsRunning
        {
            get { return _listener.Status == TaskStatus.Running; }
        }
    }
}