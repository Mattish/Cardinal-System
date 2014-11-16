using System.Collections.Concurrent;
using Cardinal_System_Shared;
using Cardinal_System_Shared.Dtos;

namespace Cardinal_System_Node
{
    public class CsNodeMessageCollector
    {

        private readonly ConcurrentQueue<Message> _concurrentQueue;

        public CsNodeMessageCollector()
        {
            _concurrentQueue = new ConcurrentQueue<Message>();
        }

    }
}