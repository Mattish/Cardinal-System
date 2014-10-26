using System;
using System.Linq;
using System.Net;

namespace Cardinal_System_Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var portNumber = 25250;
            if (args.Length > 0)
                portNumber = int.Parse(args[0]);
            Console.WriteLine("Listening on port {0}", portNumber);
            var node = new CsNode(IPAddress.Loopback, 25251, portNumber);
            node.Start();
            Console.ReadKey();
        }
    }
}
