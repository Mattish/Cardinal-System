using System.Threading;
using System.Threading.Tasks;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_Common
{
    public class Heartbeater
    {
        private ComponentConnection _componentConnection;
        private readonly int _secondsPerHeartbeat;

        private bool _isRunning;
        private Task _heartbeatTask;
        private ManualResetEventSlim _manualResetEventSlim; // TODO: what do with this?

        public Heartbeater(ComponentConnection componentConnection, int secondsPerHeartbeat)
        {
            _isRunning = false;
            _componentConnection = componentConnection;
            _secondsPerHeartbeat = secondsPerHeartbeat;
            _heartbeatTask = new Task(HeartbeatTaskLoop);
            _manualResetEventSlim = new ManualResetEventSlim();
        }

        public void Start()
        {
            _isRunning = true;
            _heartbeatTask.Start();
        }

        public void Stop()
        {
            _isRunning = false;
            _manualResetEventSlim.Set();
        }

        public void ReassignComponentConnection(ComponentConnection componentConnection)
        {
            _componentConnection = componentConnection;
        }

        private void HeartbeatTaskLoop()
        {
            while (_isRunning)
            {
                if (_componentConnection.IsConnected)
                {
                    var heartbeat = new Heartbeat();
                    heartbeat.TargetComponent = -1;
                    _componentConnection.SendMessage(heartbeat);
                }
                //Thread.Sleep(1000 * _secondsPerHeartbeat); // TODO: Make nicer
            }
        }
    }
}