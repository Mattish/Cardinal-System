namespace Cardinal_System_Shared
{
    public class ComponentMessage : Message
    {
        public ComponentMessage(MessageType type)
            : base(type)
        {
        }
    }

    public class RegisterEntityInterestMessage : Message
    {
        public RegisterEntityInterestMessage()
            : base(MessageType.RegisterEntityInterest)
        {
        }
    }
}