using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Circuit
{
    public class CsCircuitSender
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _received;
        private readonly Task _senderTask;

        public CsCircuitSender(TcpClient client, ConcurrentQueue<MessageDto> received)
        {
            _client = client;
            _received = received;
            _senderTask = new Task(DoSending);
        }

        public void Start()
        {
            _senderTask.Start();
        }

        private void DoSending()
        {
            var stream = _client.GetStream();
            using (var textWriter = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8)))
            {
                while (_client.Connected)
                {
                    int batchedCount = 0;
                    var messageDtoArray = new MessageDtoArray();
                    while (!_received.IsEmpty && batchedCount < 17)
                    {
                        MessageDto messageDto;
                        bool couldDequeue = _received.TryDequeue(out messageDto);
                        if (couldDequeue)
                        {
                            messageDtoArray.MessageDtos.Add(messageDto);
                            batchedCount++;
                        }
                    }
                    if (batchedCount > 0)
                    {
                        string json = JsonConvert.SerializeObject(messageDtoArray);
                        textWriter.WriteRaw(json);
                        Console.WriteLine("Sending DtoArray length:{0}", messageDtoArray.MessageDtos.Count());
                    }
                }
            }
        }

    }
}