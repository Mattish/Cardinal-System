using System;

namespace Cardinal_System_Shared
{
    public class PhysicalCreateMessage : Message
    {
        public long GlobalId;
        public Tuple<int, int> InitialPosition;
        public PhysicalCreateMessage(long id, Tuple<int, int> initialPosition)
        {
            GlobalId = id;
            InitialPosition = initialPosition;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.PhysicalEntityCreate;
        }
    }
}