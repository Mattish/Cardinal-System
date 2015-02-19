using System;

namespace Cardinal_System_Test.DelayedActionTest
{
    public class DelayAction : IComparable<DelayAction>
    {
        private readonly string _name;
        private readonly int _delayActionNumber = ++_internalNumber;
        private static int _internalNumber;

        public DateTime TimeSent;
        public Action Action;
        public int DelayActionNumber { get { return _delayActionNumber; } }

        public string Name
        {
            get { return _name; }
        }

        public DelayAction(string name)
        {
            _name = name;
        }

        public int CompareTo(DelayAction other)
        {
            if (TimeSent.CompareTo(other.TimeSent) == -1)
            {
                return -1;
            }
            if (TimeSent.CompareTo(other.TimeSent) == 0)
            {
                return DelayActionNumber.CompareTo(other.DelayActionNumber);
            }
            return 1;
        }
    }
}