using Cardinal_System_Common;

namespace Cardinal_System_HeathCliff.InternalMessage
{
    public class ComponentIdRequest
    {
        public ComponentConnection ComponentConnection { get; private set; }

        public ComponentIdRequest(ComponentConnection componentConnection)
        {
            ComponentConnection = componentConnection;
        }
    }
}