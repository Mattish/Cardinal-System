using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    internal class CsNodeMessageListener
    {
        private readonly TcpClient _client;
        private readonly ConcurrentQueue<MessageDto> _received;
        private Task _listener;

        public CsNodeMessageListener(TcpClient client, ConcurrentQueue<MessageDto> received)
        {
            _received = received;
            _client = client;
        }

        public void Start()
        {
            _listener = new Task(() =>
            {
                using (_client)
                {
                    _client.Client.ReceiveBufferSize = short.MaxValue;
                    var stream = _client.GetStream();
                    using (var textReader = new JsonTextReader(new StreamReader(stream, Encoding.UTF8, false)))
                    {
                        while (_client.Connected)
                        {
                            var result = textReader.ReadAsString();
                            var entityDtoArray = JsonConvert.DeserializeObject<MessageDtoArray>(result);
                            foreach (var entityDto in entityDtoArray.MessageDtos)
                            {
                                _received.Enqueue(entityDto);
                            }
                        }
                    }
                }
            });
            _listener.Start();
        }



        //private readonly ConcurrentDictionary<int, Entity> _entities;
        //private readonly ConcurrentQueue<MessageWrapper> _received;
        //private readonly Task _processEntitiesTask;
        //private readonly CsNodeListenerOld _listenerOld;
        //public CsNodeMessageListener(int port, ConcurrentDictionary<int, Entity> entities)
        //{
        //    _entities = entities;
        //    _received = new ConcurrentQueue<MessageWrapper>();
        //    _processEntitiesTask = new Task(ProcessEntities);
        //    _processEntitiesTask.Start();
        //    _listener = new CsNodeListener(port, _received);
        //}

        //public void Start()
        //{
        //    _listener.Start();
        //}

        //private void ProcessEntities()
        //{
        //    while (true)
        //    {
        //        while (!_received.IsEmpty && _value <= 16)
        //        {
        //            MessageWrapper resultMessage;
        //            if (_received.TryDequeue(out resultMessage))
        //            {
        //                ProcessEntity(resultMessage);
        //            }
        //            _value++;
        //            _total++;
        //        }
        //        if (_value > 0)
        //        {
        //            Console.WriteLine("CsNodeListener did {0} processes, total:{1}", _value, _total);
        //            _value = 0;
        //        }
        //    }
        //}

        //private void ProcessEntity(MessageWrapper resultMessage)
        //{
        //    switch (resultMessage.Type)
        //    {
        //        case MessageType.PhysicalEntityPosition:
        //            if (_entities.ContainsKey(resultMessage.Message.SourceId))
        //                _entities[resultMessage.Message.SourceId].UpdateState(resultMessage.Message, resultMessage.Type);
        //            break;
        //        case MessageType.PhysicalEntityCreate:
        //            var entityChangeCreate = resultMessage.Message as PhysicalCreateMessage;
        //            var newEntity = new PhysicalEntity(entityChangeCreate.GlobalId, entityChangeCreate.InitialPosition);
        //            _entities.TryAdd(newEntity.GlobalId, newEntity);
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}
    }
}