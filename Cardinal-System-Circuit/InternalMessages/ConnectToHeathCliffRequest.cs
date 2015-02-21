namespace Cardinal_System_Circuit.InternalMessages
{
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
}