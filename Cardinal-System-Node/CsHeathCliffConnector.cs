using System;
using System.Configuration;
using System.Net;
using System.Threading;
using Cardinal_System_Common;
using Cardinal_System_Shared.Dtos;
using Cardinal_System_Shared.Dtos.Component;
using Newtonsoft.Json;

namespace Cardinal_System_Node
{
    public static class CsHeathCliffConnector
    {
        private static CsComponentConnection _connection;
        private static CsComponentRequest _requestInfo;
        private static ManualResetEvent _resetEvent;
        public static CsComponentRequest GetNodeInfo(int hostingPort)
        {
            _resetEvent = new ManualResetEvent(false);
            var hcIpAddress = ConfigurationManager.AppSettings["HCIPAddress"];
            var hcPort = int.Parse(ConfigurationManager.AppSettings["HCPort"]);
            _requestInfo = null;
            _connection = new CsComponentConnection(IPAddress.Parse(hcIpAddress), hcPort, GotMessage, Disconnect);
            _connection.Start();
            _connection.SendMessage(new InitialInfoRequestMessage
            {
                Type = MessageType.InitialInfoRequest,
                MessageObj = new CsComponentRequestInfo
                {
                    IpAddress = "127.0.0.1",
                    Port = hostingPort
                }
            }.ToDto());

            if (!_resetEvent.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Console.WriteLine("Timeout while getting response from HeathCliff");
            }
            return _requestInfo;
        }

        private static void Disconnect(CsComponentConnection connection)
        {

        }

        private static void GotMessage(MessageDto dto, CsComponentConnection connection)
        {
            if (dto.Type == MessageType.InitialInfoResponse)
            {
                var responseInfo = JsonConvert.DeserializeObject<CsComponentRequest>(dto.MessageObj);
                if (responseInfo != null)
                {
                    _requestInfo = responseInfo;
                    _resetEvent.Set();
                }
            }
        }

        public static CsComponentConnection GetConnection()
        {
            return _connection;
        }
    }
}