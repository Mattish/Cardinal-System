using System.Collections.Generic;

namespace Cardinal_System_Shared
{
    public abstract class EntityChange
    {
        public int SourceId;
        public int TargetId;

        public abstract EntityChangeType GetEntityChangeType();
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
        public IEnumerable<EntityChangeDto> EntityChangeDtos { get; set; }
    }

    public class EntityChangeWrapper
    {
        public EntityChangeType Type { get; set; }
        public EntityChange EntityChange { get; set; }
    }
}