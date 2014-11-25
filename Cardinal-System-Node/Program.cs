using System;
using System.Configuration;
using System.Threading;
using Cardinal_System_Circuit;
using Cardinal_System_Common;
using Cardinal_System_HeathCliff;

namespace Cardinal_System_Node
{
    class Program
    {
        static void Main(string[] args)
        {
            CsNode node = null;
            if (args.Length == 1)
            {
                if (args[0] == "HeathCliff")
                {
                    node = new CsHeathCliff(int.Parse(ConfigurationManager.AppSettings["HCPort"]));
                    Console.WriteLine("Starting up as HeathCliff");
                }
            }
            else
            {
                var hostingPort = int.Parse(ConfigurationManager.AppSettings["Port"]);
                var nodeInfo = CsHeathCliffConnector.GetNodeInfo(hostingPort);
                if (nodeInfo != null)
                {
                    if (nodeInfo.NodeType == CsNodeType.Circuit)
                    {
                        node = new CsCircuit(hostingPort);
                        Console.WriteLine("Starting up as Circuit");
                    }
                }
            }

            if (node != null)
            {
                node.Start();
                while (node.IsRunning)
                {
                    Thread.Sleep(1);
                }
            }

            // Get HeathCliff(HC) location from config
            // Connect to HC
            // Request ID, NodeType and Circuit Location(s)
            // Receive ID, NodeType and Circuit Location(s)
            // Drop
            // Start up job if given
            // Connect to Circuit(s)
            // Broadcast Online
        }
    }
}
