using System;

namespace Cardinal_System_Shared.Dtos.Entity
{
    public class PhysicalCreateMessage : Message
    {
        public long GlobalId;
        public Tuple<int, int> InitialPosition;
        public PhysicalCreateMessage(long id, Tuple<int, int> initialPosition)
            : base(MessageType.PhysicalEntityCreate)
        {
            GlobalId = id;
            InitialPosition = initialPosition;
        }
    }
}