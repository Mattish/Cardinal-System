namespace Cardinal_System_Circuit.InternalMessages
{
    public class DisconnectFromHeathCliff
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public long ComponentId { get; private set; }

        public bool ConnectToOther
        {
            get { return IpAddress != null; }
        }

        public DisconnectFromHeathCliff(string ipAddress, int port, long componentId)
        {
            IpAddress = ipAddress;
            Port = port;
            ComponentId = componentId;
        }
    }
}