using Cardinal_System_Shared;

namespace Cardinal_System_Common
{
    public class WrappedMessage
    {
        public ComponentConnection ComponentConnection { get; private set; }
        public Message Message { get; private set; }

        public WrappedMessage(ComponentConnection componentConnection, Message message)
        {
            ComponentConnection = componentConnection;
            Message = message;
        }
    }
}