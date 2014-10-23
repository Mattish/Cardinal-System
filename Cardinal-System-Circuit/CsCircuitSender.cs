using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Circuit
{
    public class CsCircuitSender
    {
        private readonly ConcurrentQueue<MessageDto> _received;
        private readonly Dictionary<int, IPEndPoint> _componentAddresses;
        private readonly Dictionary<int, List<int>> _componentsInterestInEntity;
        private readonly Task _senderTask;

        public CsCircuitSender(ConcurrentQueue<MessageDto> received)
        {
            _componentAddresses = new Dictionary<int, IPEndPoint>();
            _componentsInterestInEntity = new Dictionary<int, List<int>>();
            _received = received;
            _senderTask = new Task(DoSending);
        }

        public void Start()
        {
            _senderTask.Start();
        }

        private void DoSending()
        {
            var udpSender = new UdpClient();
            int i = 0;
            while (true)
            {
                while (!_received.IsEmpty)
                {
                    MessageDto messageDto;
                    bool couldDequeue = _received.TryDequeue(out messageDto);
                    //Batch this shit better
                    if (couldDequeue)
                    {
                        int targetEntity = messageDto.TargetId;
                        if (_componentsInterestInEntity.ContainsKey(targetEntity) && _componentsInterestInEntity[targetEntity].Count > 0)
                        {
                            foreach (var componentInterestInEntity in _componentsInterestInEntity[targetEntity])
                            {
                                var messageArrayDto = new MessageDtoArray();
                                messageArrayDto.MessageDtos = new[] { messageDto };
                                string json = JsonConvert.SerializeObject(messageArrayDto);
                                byte[] buffer = Encoding.UTF8.GetBytes(json);
                                var endPoint = _componentAddresses[componentInterestInEntity];
                                udpSender.Send(buffer, buffer.Length, endPoint);
                                Console.WriteLine("#{2} Sending Dto targetId:{0} to component:{1} of type:{3}", messageDto.TargetId, componentInterestInEntity, i++, messageDto.Type);
                            }
                        }
                    }
                    Thread.Sleep(1);
                }
            }
        }

        public void RegisterComponentAddress(int componentId, IPEndPoint endpoint)
        {
            _componentAddresses.Add(componentId, endpoint);
        }

        public void RegisterComponentInterestInEntity(int componentId, int entityId)
        {
            if (!_componentsInterestInEntity.ContainsKey(componentId))
                _componentsInterestInEntity.Add(entityId, new List<int>());
            _componentsInterestInEntity[entityId].Add(componentId);
        }
    }
}