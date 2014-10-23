using System;

namespace Cardinal_System_Shared
{
    public class PhysicalMovementMessage : Message
    {
        public Tuple<int, int> NewPosition;
        public override MessageType GetMessageType()
        {
            return MessageType.PhysicalEntityPosition;
        }
    }
}