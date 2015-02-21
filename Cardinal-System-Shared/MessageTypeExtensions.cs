using System;
using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared
{
    public static class MessageTypeExtensions
    {
        public static MessageFamily GetMessageFamily(this MessageType type)
        {
            switch (type)
            {
                case MessageType.HeathCliffOrderConnect:
                case MessageType.HeathCliffOrderDisconnect:
                    return MessageFamily.Component;
                default:
                    return MessageFamily.Unknown;
            }
        }

        public static Type GetConcreteType(this MessageType type)
        {
            switch (type)
            {
                case MessageType.HeathCliffOrderConnect:
                    return typeof (HeathCliffOrderConnect);
                case MessageType.HeathCliffOrderDisconnect:
                    return typeof(HeathCliffOrderDisconnect);
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}