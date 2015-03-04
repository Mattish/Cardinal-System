namespace Cardinal_System_Common.MessageNetworking
{
    public class ComponentConnectionDisconnect
    {
        public ComponentConnection ComponentConnection { get;private set; }

        public ComponentConnectionDisconnect(ComponentConnection componentConnection)
        {
            ComponentConnection = componentConnection;
        }
    }
}
