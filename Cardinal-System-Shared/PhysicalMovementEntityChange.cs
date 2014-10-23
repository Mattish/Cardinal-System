using System;

namespace Cardinal_System_Shared
{
    public class PhysicalMovementEntityChange : EntityChange
    {
        public Tuple<int, int> NewPosition;
        public override EntityChangeType GetEntityChangeType()
        {
            return EntityChangeType.PhysicalPosition;
        }
    }
}