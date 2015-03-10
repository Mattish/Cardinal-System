using System;
using System.Configuration;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;

namespace Cardinal_System_Circuit
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            MessageHubV2.Start();
            ComponentSettings.ComponentType = ComponentType.Circuit;
            var port = int.Parse(ConfigurationManager.AppSettings["CircuitPort"]);
            var adress = ConfigurationManager.AppSettings["CircuitAddress"];
            Console.WriteLine("Starting up Circuit");
            Console.WriteLine("Give extra port number");
            var lineIn = Console.ReadLine();
            int portExtra = 0;
            if (lineIn != null)
            {
                try
                {
                    portExtra = int.Parse(lineIn);
                }
                catch (Exception)
                {
                    Console.WriteLine("no port given");
                    return;
                }
            }
            var circuit = new Circuit(adress, port + portExtra);
            circuit.Start();
            Console.WriteLine("Starting up as Circuit on port {0}", port + portExtra);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
