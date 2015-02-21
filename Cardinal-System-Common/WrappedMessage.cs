using Cardinal_System_Shared;

namespace Cardinal_System_Common
{
    public class WrappedMessage
    {
        public CsComponentConnection CsComponentConnection { get; private set; }
        public Message Message { get; private set; }

        public WrappedMessage(CsComponentConnection csComponentConnection, Message message)
        {
            CsComponentConnection = csComponentConnection;
            Message = message;
        }
    }
}