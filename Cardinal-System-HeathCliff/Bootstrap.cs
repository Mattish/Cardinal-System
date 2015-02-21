using System;
using System.Configuration;
using Cardinal_System_Common;
using Cardinal_System_Common.MessageNetworking;

namespace Cardinal_System_HeathCliff
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            CsComponentSettings.ComponentId = -1;
            MessageHubV2.Start();
            var port = int.Parse(ConfigurationManager.AppSettings["HCPort"]);
            var hc = new CsHeathCliff(port);
            hc.Start();
            Console.WriteLine("Starting up HeathCliff");
            Console.ReadKey();
        }
    }
}
