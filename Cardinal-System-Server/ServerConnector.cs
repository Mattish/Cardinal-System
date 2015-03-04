using System.Net;
using Cardinal_System_Common;

namespace Cardinal_System_Server
{
    class ServerConnector : ComponentConnection
    {
        public ServerConnector(string initialAddress, int port)
            : base(initialAddress, port)
        {

        }

        public long ComponentId { get; set; }
    }
}