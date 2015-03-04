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
            Console.WriteLine("Starting up as Circuit");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
