using System;
using System.Linq;

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
            var node = new CsNode(portNumber);
            if (portNumber == 25250)
                node.StartTest("127.0.0.1", 25251);
            else
                node.Start();
            Console.ReadKey();
        }
    }
}
