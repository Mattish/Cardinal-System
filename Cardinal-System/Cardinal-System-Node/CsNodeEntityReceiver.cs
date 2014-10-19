using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNodeEntityReceiver
    {
        private readonly ConcurrentQueue<EntityChangeWrapper> _received;
        private readonly Task _processEntitiesTask;
        private readonly CsNodeListener _listener;

        int _value;
        DateTime _lastNow = DateTime.Now;

        public CsNodeEntityReceiver(int port)
        {
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
                }
            }
        }

        private void ProcessEntity(EntityChangeWrapper resultEntityChange)
        {
            if ((DateTime.Now - _lastNow).Duration() > TimeSpan.FromSeconds(1))
            {
                Console.WriteLine("Did {0} processes", _value);
                _value = 0;
                _lastNow = DateTime.Now;
            }
            _value++;

            switch (resultEntityChange.Type)
            {
                case EntityChangeType.PhysicalPosition:
                    break;
                case EntityChangeType.PhysicalCreate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}