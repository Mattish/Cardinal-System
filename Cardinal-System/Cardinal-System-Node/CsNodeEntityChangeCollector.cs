using System.Collections.Concurrent;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    public class CsNodeEntityChangeCollector
    {

        private readonly ConcurrentQueue<EntityChange> _concurrentQueue;

        public CsNodeEntityChangeCollector()
        {
            _concurrentQueue = new ConcurrentQueue<EntityChange>();
        }

    }
}