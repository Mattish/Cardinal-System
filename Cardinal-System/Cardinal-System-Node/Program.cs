using System;

namespace Cardinal_System_Node
{
    class Program
    {
        static void Main(string[] args)
        {
            var portNumber = 25251;
            if (args.Length > 0)
                portNumber = int.Parse(args[0]);

            var node = new CsNode(portNumber);
            node.Start();
            Console.ReadKey();
        }
    }
}
