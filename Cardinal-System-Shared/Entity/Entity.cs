using Cardinal_System_Shared.Dtos;

namespace Cardinal_System_Shared.Entity
{
    public abstract class Entity
    {
        private static long _globalId;
        private static long _nodeId;

        public string EntityData = "EntityData";
        public EntityId Id { get; protected set; }
        public bool Changed { get; private set; }

        protected Entity()
        {
            Id = new EntityId(_nodeId, _globalId++);
        }

        protected Entity(EntityId entityId)
        {
            Id = entityId;
        }

        protected void StateChange()
        {
            Changed = true;
        }

        public abstract void UpdateState(object updateWith, MessageType changeType);
        public abstract EntityType GetEntityType();

        public static void SetNodeId(long nodeId)
        {
            _nodeId = nodeId;
        }
    }

    public struct EntityId
    {
        public long ComponentId { get; private set; }
        public long Id { get; private set; }

        public EntityId(long componentId, long id)
            : this()
        {
            Id = id;
            ComponentId = componentId;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", ComponentId, Id);
        }
    }

    public enum EntityType
    {
        BasicPhysicalEntity,
        BasicInformationEntity
    }
}