using Cardinal_System_Shared.Dtos;

namespace Cardinal_System_Shared.Entity
{
    public abstract class Entity
    {
        private static long _globalId;

        public string EntityData = "EntityData";


        public long GlobalId { get; protected set; }
        public bool Changed { get; private set; }

        protected Entity()
        {
            GlobalId = _globalId++;
        }

        protected Entity(bool createdAsReceive = false)
        {
            if (!createdAsReceive)
                GlobalId = _globalId++;
        }

        protected void StateChange()
        {
            Changed = true;
        }

        public abstract void UpdateState(object updateWith, MessageType changeType);
        public abstract EntityType GetEntityType();
    }

    public enum EntityType
    {
        BasicPhysicalEntity,
        BasicInformationEntity
    }
}