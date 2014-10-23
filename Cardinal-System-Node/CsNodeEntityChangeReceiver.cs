using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNodeEntityChangeReceiver
    {
        private readonly ConcurrentDictionary<int, Entity> _entities;
        private readonly ConcurrentQueue<EntityChangeWrapper> _received;
        private readonly Task _processEntitiesTask;
        private readonly CsNodeListener _listener;

        private int _value;
        private int _total;
        DateTime _lastNow = DateTime.Now;

        public CsNodeEntityChangeReceiver(int port, ConcurrentDictionary<int, Entity> entities)
        {
            _entities = entities;
            _received = new ConcurrentQueue<EntityChangeWrapper>();
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
                while (!_received.IsEmpty)
                {
                    EntityChangeWrapper resultEntityChange;
                    if (_received.TryDequeue(out resultEntityChange))
                    {
                        ProcessEntity(resultEntityChange);
                    }
                    _value++;
                    _total++;
                }
                if (_value != 0)
                    Console.WriteLine("Did {0} processes, total:{1}", _value, _total);
                _value = 0;
            }
        }

        private void ProcessEntity(EntityChangeWrapper resultEntityChange)
        {
            switch (resultEntityChange.Type)
            {
                case EntityChangeType.PhysicalPosition:
                    if (_entities.ContainsKey(resultEntityChange.EntityChange.SourceId))
                        _entities[resultEntityChange.EntityChange.SourceId].UpdateState(resultEntityChange.EntityChange, resultEntityChange.Type);
                    break;
                case EntityChangeType.PhysicalCreate:
                    var entityChangeCreate = resultEntityChange.EntityChange as PhysicalCreateEntityChange;
                    var newEntity = new PhysicalEntity(entityChangeCreate.GlobalId, entityChangeCreate.InitialPosition);
                    _entities.TryAdd(newEntity.GlobalId, newEntity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}