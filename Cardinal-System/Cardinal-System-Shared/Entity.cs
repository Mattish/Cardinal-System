namespace Cardinal_System_Shared
{
    public abstract class Entity : ChangeableState
    {
        public string EntityData = "EntityData";

        private readonly int _globalId;
        public int GlobalId { get; private set; }

        public abstract EntityType GetEntityType();

        protected Entity()
        {
            GlobalId = _globalId++;
        }
    }

    public enum EntityType
    {
        BasicPhysicalEntity,
        BasicInformationEntity
    }
}