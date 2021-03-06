﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public class MessageSender : IAsyncRunnable
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _senderQueue;
        private readonly Action _disconnectAction;
        private readonly Task _senderTask;
        private readonly ManualResetEventSlim _manualResetEventSlim;

        public MessageSender(TcpClient client, ConcurrentQueue<MessageDto> senderQueue, Action disconnectAction,
            ManualResetEventSlim manualResetEventSlim)
        {
            _client = client;
            _senderQueue = senderQueue;
            _disconnectAction = disconnectAction;
            _manualResetEventSlim = manualResetEventSlim;
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
                int batchedCount;
                var messageDtoArray = new MessageDtoArray
                {
                    Dtos = new List<MessageDto>()
                };
                while (_client.Connected)
                {
                    batchedCount = 0;
                    messageDtoArray.Dtos.Clear();

                    while (!_senderQueue.IsEmpty && batchedCount < 16)
                    {
                        MessageDto messageDto;
                        bool couldDequeue = _senderQueue.TryDequeue(out messageDto);

                        if (couldDequeue)
                        {
                            messageDtoArray.Dtos.Add(messageDto);
                            batchedCount++;
                            dequeueAmount++;
                        }
                    }
                    if (batchedCount > 0)
                    {
                        sendTotal += batchedCount;
                        string json = JsonConvert.SerializeObject(messageDtoArray);
                        //Console.WriteLine("sendTotal: {0}", sendTotal);
                        json += "\r\n";
                        textWriter.WriteRaw(json);
                        textWriter.Flush();
                    }
                    if (_senderQueue.IsEmpty)
                        _manualResetEventSlim.Wait(TimeSpan.FromSeconds(1));
                    _manualResetEventSlim.Reset();
                }
                Console.WriteLine("sender disconnected");
            }
            catch (Exception e)
            {
                Console.WriteLine("Client sender error :( {0}", e.Message);
            }
            _disconnectAction();
        }

        public bool IsRunning
        {
            get { return _senderTask.Status == TaskStatus.Running; }
        }
    }
}