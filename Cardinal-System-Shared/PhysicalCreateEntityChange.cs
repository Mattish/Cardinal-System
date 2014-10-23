using System;

namespace Cardinal_System_Shared
{
    public class PhysicalCreateEntityChange : EntityChange
    {
        public int GlobalId;
        public Tuple<int, int> InitialPosition;
        public PhysicalCreateEntityChange(int id, Tuple<int, int> initialPosition)
        {
            GlobalId = id;
            InitialPosition = initialPosition;
        }

        public override EntityChangeType GetEntityChangeType()
        {
            return EntityChangeType.PhysicalCreate;
        }
    }
}