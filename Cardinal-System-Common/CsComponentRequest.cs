namespace Cardinal_System_Common
{
    public class CsComponentRequest
    {
        public long Id;
        public CsNodeType NodeType;
        public CsComponentRequestInfo[] Circuits;
    }

    public class CsComponentRequestInfo
    {
        public string IpAddress;
        public int Port;
        public long Id;
    }
}