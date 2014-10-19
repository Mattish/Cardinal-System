using System;

namespace Cardinal_System_Shared
{
    public abstract class EntityChange
    {
        public int SourceId;
        public int TargetId;

        public abstract EntityChangeType GetEntityChangeType();
    }

    public class PhysicalMovementEntityChange : EntityChange
    {
        public Tuple<int, int> PositionChange;
        public override EntityChangeType GetEntityChangeType()
        {
            return EntityChangeType.PhysicalPosition;
        }
    }

    public enum EntityChangeType
    {
        PhysicalPosition,
        PhysicalCreate
    }

    public class EntityChangeDto
    {
        public EntityChangeType Type { get; set; }
        public string EntityChange { get; set; }
    }

    public class EntityChangeDtoArray
    {
        public EntityChangeDto[] EntityChangeDtos { get; set; }
    }

    public class EntityChangeWrapper
    {
        public EntityChangeType Type { get; set; }
        public EntityChange EntityChange { get; set; }
    }
}