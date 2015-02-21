using System;
using System.Configuration;
using Cardinal_System_Common.MessageNetworking;

namespace Cardinal_System_Circuit
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            MessageHubV2.Start();
            var port = int.Parse(ConfigurationManager.AppSettings["CircuitPort"]);
            var adress = ConfigurationManager.AppSettings["CircuitAddress"];
            var circuit = new CsCircuit(adress, port);
            circuit.Start();
            Console.WriteLine("Starting up as Circuit");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
