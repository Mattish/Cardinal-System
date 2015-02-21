namespace Cardinal_System_Shared.Component
{
    public abstract class ComponentMessage : Message
    {
        protected ComponentMessage(MessageType type)
            : base(type) { }
    }
}