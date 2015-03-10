using System;
using System.Configuration;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;

namespace Cardinal_System_Server
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            MessageHubV2.Start();
            ComponentSettings.ComponentType = ComponentType.Server;
            var area = new Area("Default", new Tuple<double, double>(10, 10), new Tuple<double, double>(-10, -10));
            var address = ConfigurationManager.AppSettings["ServerAddress"];
            var port = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            Console.WriteLine("Starting up Server");
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
            var server = new Server(area, address, port + portExtra);
            server.Start();
            Console.WriteLine("Starting up Server in port {0}", port + portExtra);
            Console.ReadKey();
        }
    }
}
