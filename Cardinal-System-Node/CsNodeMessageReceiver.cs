using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNodeMessageReceiver
    {
        private readonly ConcurrentDictionary<int, Entity> _entities;
        private readonly ConcurrentQueue<MessageWrapper> _received;
        private readonly Task _processEntitiesTask;
        private readonly CsNodeListener _listener;

        private int _value;
        private int _total;
        DateTime _lastNow = DateTime.Now;

        public CsNodeMessageReceiver(int port, ConcurrentDictionary<int, Entity> entities)
        {
            _entities = entities;
            _received = new ConcurrentQueue<MessageWrapper>();
            _processEntitiesTask = new Task(ProcessEntities);
            _processEntitiesTask.Start();
            _listener = new CsNodeListener(port, _received);
        }

        public void Start()
        {
            _listener.Start();
        }

        private void ProcessEntities()
        {
            while (true)
            {
                Thread.Sleep(1);
                while (!_received.IsEmpty && _value != 10)
                {
                    MessageWrapper resultMessage;
                    if (_received.TryDequeue(out resultMessage))
                    {
                        ProcessEntity(resultMessage);
                    }
                    _value++;
                    _total++;
                }
                if (_value == 10)
                {
                    Console.WriteLine("Did {0} processes, total:{1}", _value, _total);
                    _value = 0;
                }
            }
        }

        private void ProcessEntity(MessageWrapper resultMessage)
        {
            switch (resultMessage.Type)
            {
                case MessageType.PhysicalEntityPosition:
                    if (_entities.ContainsKey(resultMessage.Message.SourceId))
                        _entities[resultMessage.Message.SourceId].UpdateState(resultMessage.Message, resultMessage.Type);
                    break;
                case MessageType.PhysicalEntityCreate:
                    var entityChangeCreate = resultMessage.Message as PhysicalCreateMessage;
                    var newEntity = new PhysicalEntity(entityChangeCreate.GlobalId, entityChangeCreate.InitialPosition);
                    _entities.TryAdd(newEntity.GlobalId, newEntity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}