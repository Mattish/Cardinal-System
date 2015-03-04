namespace Cardinal_System_Common
{
    public class ComponentRequest
    {
        public long Id;
        public NodeType NodeType;
        public ComponentRequestInfo[] Circuits;
    }

    public class ComponentRequestInfo
    {
        public string IpAddress;
        public int Port;
        public long Id;
    }
}