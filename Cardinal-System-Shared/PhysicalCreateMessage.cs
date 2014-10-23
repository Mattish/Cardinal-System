using System;

namespace Cardinal_System_Shared
{
    public class PhysicalCreateMessage : Message
    {
        public int GlobalId;
        public Tuple<int, int> InitialPosition;
        public PhysicalCreateMessage(int id, Tuple<int, int> initialPosition)
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