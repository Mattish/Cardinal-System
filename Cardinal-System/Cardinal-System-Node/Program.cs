using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Cardinal_System_Shared;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public class CsNodeListener
    {
        private readonly int _port;
        private Task _listener;

        public CsNodeListener(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _listener = new Task(async () =>
            {
                using (var udpListener = new UdpClient(_port))
                {
                    while (true)
                    {
                        var udpResult = await udpListener.ReceiveAsync();
                        byte[] bytesArray = udpResult.Buffer;
                        string json = Encoding.UTF8.GetString(bytesArray);
                        TestData someData = JsonConvert.DeserializeObject<TestData>(json);
                        Console.WriteLine("Received SomeData:{0}", someData.SomeData);
                    }
                }
            });
            _listener.Start();
        }
    }

    public static class CsNodeSender
    {
        private static UdpClient _udpSender;
        public static void Send<T>(T objectToSend, string host, int port)
        {
            if (_udpSender == null)
                _udpSender = new UdpClient();

            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objectToSend));
            _udpSender.Send(buffer, buffer.Length, host, port);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var portNumber = 25251;
            if (args.Length > 0)
                portNumber = int.Parse(args[0]);

            CsNodeListener listener = new CsNodeListener(portNumber);
            listener.Start();

            var someData = new TestData { SomeData = "SomeDataString" };
            CsNodeSender.Send(someData, "localhost", portNumber);
            CsNodeSender.Send(someData, "localhost", portNumber);
            CsNodeSender.Send(someData, "localhost", portNumber);
            Console.ReadKey();
        }
    }
}
