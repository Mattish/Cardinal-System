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

    public class UnregisterEntityOwnerMessage : Message
    {
        public UnregisterEntityOwnerMessage()
            : base(MessageType.UnregisterEntityOwner)
        {
        }
    }

    public class RegisterEntityOwnerMessage : Message
    {
        public RegisterEntityOwnerMessage()
            : base(MessageType.RegisterEntityOwner)
        {
        }
    }

    public class UnregisterEntityInterestMessage : Message
    {
        public UnregisterEntityInterestMessage()
            : base(MessageType.UnregisterEntityInterest)
        {
        }
    }

    public class RegisterWithCircuitMessage : Message
    {
        public RegisterWithCircuitMessage()
            : base(MessageType.RegisterWithCircuit)
        {
        }
    }

    public class UnregisterWithCircuitMessage : Message
    {
        public UnregisterWithCircuitMessage()
            : base(MessageType.UnregisterWithCircuit)
        {
        }
    }
}