namespace Cardinal_System_Common.MessageNetworking
{
    public class ComponentConnectionDisconnect
    {
        public CsComponentConnection CsComponentConnection { get;private set; }

        public ComponentConnectionDisconnect(CsComponentConnection csComponentConnection)
        {
            CsComponentConnection = csComponentConnection;
        }
    }
}
