namespace Cardinal_System_Shared.Dtos.Component
{
    public class ComponentMessage : Message
    {
        public ComponentMessage(MessageType type)
            : base(type)
        {
        }
    }

    public class InitialInfoRequestMessage : Message
    {
        public InitialInfoRequestMessage()
            : base(MessageType.InitialInfoRequest)
        {
        }
    }

    public class InitialInfoResponseMessage : Message
    {
        public InitialInfoResponseMessage()
            : base(MessageType.InitialInfoResponse)
        {
        }
    }
}