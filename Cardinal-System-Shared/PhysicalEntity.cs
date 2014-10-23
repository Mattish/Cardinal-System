using System;

namespace Cardinal_System_Shared
{
    public class PhysicalEntity : Entity
    {
        private Tuple<int, int> _xyPos;

        public Tuple<int, int> Position
        {
            get { return _xyPos; }
        }

        public PhysicalEntity()
        {
            _xyPos = new Tuple<int, int>(0, 0);
        }

        public PhysicalEntity(int id, Tuple<int, int> initialTuple)
        {
            GlobalId = id;
            _xyPos = initialTuple;
        }

        public override void UpdateState(object updateWith, MessageType changeType)
        {
            switch (changeType)
            {
                case MessageType.PhysicalEntityPosition:
                    var physicalMovementEntityChange = updateWith as PhysicalMovementMessage;
                    if (physicalMovementEntityChange != null)
                        _xyPos = new Tuple<int, int>(physicalMovementEntityChange.NewPosition.Item1 + _xyPos.Item1,
                            physicalMovementEntityChange.NewPosition.Item2 + _xyPos.Item2);
                    break;
            }
        }

        public override EntityType GetEntityType()
        {
            return EntityType.BasicPhysicalEntity;
        }
    }

    public class InformationEntity : Entity
    {
        public override EntityType GetEntityType()
        {
            return EntityType.BasicInformationEntity;
        }

        public override void UpdateState(object updateWith, MessageType changeType)
        {
            throw new NotImplementedException();
        }
    }
}
