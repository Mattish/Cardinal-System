using System;
using System.Configuration;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;
using Cardinal_System_Shared;

namespace Cardinal_System_HeathCliff
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            ComponentSettings.ComponentId = -1;
            ComponentSettings.ComponentType = ComponentType.Circuit;
            MessageHubV2.Start();
            var port = int.Parse(ConfigurationManager.AppSettings["HCPort"]);
            var ipAddress = ConfigurationManager.AppSettings["HCAddress"];
            var hc = new HeathCliff(ipAddress, port);
            hc.Start();
            Console.WriteLine("Starting up HeathCliff");
            Console.ReadKey();
        }
    }
}
