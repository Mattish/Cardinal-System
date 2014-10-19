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

        public override void UpdateState(object updateWith, EntityChangeType changeType)
        {
            switch (changeType)
            {
                case EntityChangeType.PhysicalPosition:
                    var physicalMovementEntityChange = updateWith as PhysicalMovementEntityChange;
                    if (physicalMovementEntityChange != null)
                        _xyPos = new Tuple<int, int>(physicalMovementEntityChange.PositionChange.Item1 + _xyPos.Item1,
                            physicalMovementEntityChange.PositionChange.Item2 + _xyPos.Item2);
                    break;
                case EntityChangeType.PhysicalCreate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("changeType");
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

        public override void UpdateState(object updateWith, EntityChangeType changeType)
        {
            throw new NotImplementedException();
        }
    }
}
