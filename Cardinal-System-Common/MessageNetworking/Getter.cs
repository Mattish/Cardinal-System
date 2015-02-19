using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cardinal_System_Common.MessageNetworking
{
    public abstract class Getter<TRequest>
    {
        private Task _task;

        protected readonly ManualResetEventSlim ManualResetEventSlim = new ManualResetEventSlim();
        protected readonly ConcurrentQueue<TRequest> Requests = new ConcurrentQueue<TRequest>();
        protected readonly ConcurrentQueue<TRequest> ProcessingRequests = new ConcurrentQueue<TRequest>();

        public bool Running { get; private set; }

        protected Getter()
        {
            Start();
        }

        private void Start()
        {
            if (_task == null)
            {
                _task = new Task(RequestHandleLoop);
                Running = true;
                MessageHubV2.Register<TRequest>(this, AddToQueue);
                ExtraMessageRegisters();
                _task.Start();
            }
        }

        protected virtual void ExtraMessageRegisters() { }

        protected virtual void SpecificActionAfterEnqueue(TRequest request) { }

        private void AddToQueue(TRequest pageInfoRequest)
        {
            if (!Requests.Contains(pageInfoRequest))
            {
                Requests.Enqueue(pageInfoRequest);
                SpecificActionAfterEnqueue(pageInfoRequest);
                ManualResetEventSlim.Set();
            }
        }

        private void RequestHandleLoop()
        {
            while (Running)
            {
                if (Requests.IsEmpty)
                {
                    if (!SpinWait.SpinUntil(() => ManualResetEventSlim.IsSet, 1000))
                        continue;
                }
                ManualResetEventSlim.Reset();
                TRequest request;
                if (Requests.TryDequeue(out request))
                {
                    ProcessingRequests.Enqueue(request);
                    SpecificAction(request);
                }
                ProcessingRequests.TryDequeue(out request);
            }
        }

        protected abstract void SpecificAction(TRequest request);
    }
}
