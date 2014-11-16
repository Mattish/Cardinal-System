using System;

namespace Cardinal_System_Shared.Dtos.Entity
{
    public class PhysicalMovementMessage : Message
    {
        public Tuple<int, int> NewPosition;

        public PhysicalMovementMessage()
            : base(MessageType.PhysicalEntityPosition)
        {
        }
    }
}