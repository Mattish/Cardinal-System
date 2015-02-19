using System;
using System.Configuration;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;

namespace Cardinal_System_Server
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            MessageHubV2.Start();
            var area = new CsArea("Default", new Tuple<double, double>(10, 10), new Tuple<double, double>(-10, -10));
            var address = ConfigurationManager.AppSettings["ServerAddress"];
            var port = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            var server = new CsServer(area, address, port);
            Console.WriteLine("Starting up Server");
            Console.ReadKey();
        }
    }
}
