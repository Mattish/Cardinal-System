namespace Cardinal_System_Shared
{
    public abstract class Entity
    {
        private readonly int _globalId;

        public string EntityData = "EntityData";


        public int GlobalId { get; protected set; }
        public bool Changed { get; private set; }

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