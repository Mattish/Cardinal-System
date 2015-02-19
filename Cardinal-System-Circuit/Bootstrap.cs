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
            Console.WriteLine("Press any key to connect to HeathCliff");
            Console.ReadKey();
            MessageHubV2.Send(new ConnectToHeathCliffRequest(ConfigurationManager.AppSettings["HCAddress"], int.Parse(ConfigurationManager.AppSettings["HCPort"])));
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }

    public class ConnectToHeathCliffRequest
    {
        public int Port;
        public string Address;

        public ConnectToHeathCliffRequest(string address, int port)
        {
            Address = address;
            Port = port;
        }
    }

    public class ConnectToHeathCliffResponse
    {
        public bool Success;

        public ConnectToHeathCliffResponse(bool success)
        {
            Success = success;
        }
    }
}
