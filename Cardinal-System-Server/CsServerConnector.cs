using System.Net;
using Cardinal_System_Common;

namespace Cardinal_System_Server
{
    class CsServerConnector : CsComponentConnection
    {
        public CsServerConnector(string initialAddress, int initialPort)
            : base(initialAddress, initialPort)
        {

        }

        public long ComponentId { get; set; }
    }
}