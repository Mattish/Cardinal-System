using System;
using Cardinal_System_Shared;

namespace Cardinal_System_Node
{
    internal class CsNode
    {
        private readonly int _port;
        private readonly CsNodeSender _sender;
        private readonly CsNodeEntityReceiver _receiver;

        public CsNode(int port)
        {
            _port = port;
            _sender = new CsNodeSender();
            _receiver = new CsNodeEntityReceiver(port);
        }

        public void Start()
        {
            _receiver.Start();
            while (true)
            {
                _sender.Send(new PhysicalMovementEntityChange { PositionChange = new Tuple<int, int>(1, 1) }, "localhost", _port);
            }
        }
    }
}