using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Common
{
    public class CsMessageSender : IAsyncRunnable
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _senderQueue;
        private readonly Task _senderTask;

        public CsMessageSender(TcpClient client, ConcurrentQueue<MessageDto> senderQueue)
        {
            _client = client;
            _senderQueue = senderQueue;
            Console.WriteLine("Starting CsMessageSender with inital queue of {0}", _senderQueue.Count);
            _senderTask = new Task(DoSending);
        }

        public void Start()
        {
            _senderTask.Start();
        }

        private void DoSending()
        {
            var stream = _client.GetStream();
            int sendTotal = 0;
            int dequeueAmount = 0;
            try
            {
                var textWriter = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8, 8096, true));
                textWriter.Formatting = Formatting.None;
                while (_client.Connected)
                {
                    int batchedCount = 0;
                    var messageDtoArray = new MessageDtoArray
                    {
                        MessageDtos = new List<MessageDto>()
                    };

                    while (!_senderQueue.IsEmpty && batchedCount < 16)
                    {

                        MessageDto messageDto;
                        bool couldDequeue = _senderQueue.TryDequeue(out messageDto);

                        if (couldDequeue)
                        {
                            messageDtoArray.MessageDtos.Add(messageDto);
                            batchedCount++;
                            dequeueAmount++;
                        }
                    }
                    if (batchedCount > 0)
                    {
                        sendTotal += batchedCount;
                        string json = JsonConvert.SerializeObject(messageDtoArray);
                        json += "\r\n";
                        textWriter.WriteRaw(json);
                        textWriter.Flush();
                        Console.WriteLine("Sent DtoArray total:{0} dequeueAmount:{1} actualCountOfQueue:{2}",
                            sendTotal, dequeueAmount, _senderQueue.Count);
                    }
                }
                Console.WriteLine("sender disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Client sender error :( {0}", e.Message);
            }
        }

        public bool IsRunning()
        {
            return _senderTask.Status == TaskStatus.Running;
        }
    }

    public interface IAsyncRunnable
    {
        bool IsRunning();
    }
}