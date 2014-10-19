namespace Cardinal_System_Shared
{
    public abstract class ChangeableState
    {
        public bool Changed { get; private set; }

        protected void StateChange()
        {
            Changed = true;
        }

        public abstract void UpdateState(object updateWith, EntityChangeType changeType);
    }
}