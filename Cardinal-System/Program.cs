using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cardinal_System
{
    class Program
    {
        static void Main(string[] args)
        {
            var portNumber = 25251;
            if (args.Length > 0)
                portNumber = int.Parse(args[0]);

            var listenerTask = new Task(async () =>
            {
                using (var udpListener = new UdpClient(portNumber))
                {
                    while (true)
                    {
                        var udpResult = await udpListener.ReceiveAsync();
                        byte[] bytesArray = udpResult.Buffer;
                        string json = Encoding.UTF8.GetString(bytesArray);
                        SomeData someData = JsonConvert.DeserializeObject<SomeData>(json);
                        Console.WriteLine("Received SomeData:{0}", someData.StringData);
                    }
                }
            });

            listenerTask.Start();

            using (var udpListener = new UdpClient())
            {
                SomeData someData = new SomeData();
                someData.StringData = "SomeDataString";
                byte[] buffer = Encoding.UTF8.GetBytes(someData.ToString());
                udpListener.Send(buffer, buffer.Length, "localhost", portNumber);
                Thread.Sleep(1000);
                udpListener.Send(buffer, buffer.Length, "localhost", portNumber);
                Thread.Sleep(1000);
                udpListener.Send(buffer, buffer.Length, "localhost", portNumber);
            }
            Console.ReadKey();
        }
    }

    public class SomeData
    {
        public string StringData { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
