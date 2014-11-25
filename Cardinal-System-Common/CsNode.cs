namespace Cardinal_System_Common
{
    public abstract class CsNode
    {
        public static long ComponentId { get; protected set; }
        public abstract bool IsRunning { get; }

        public abstract void Start();
    }
}