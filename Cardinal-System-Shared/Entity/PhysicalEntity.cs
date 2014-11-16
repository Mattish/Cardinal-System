using System;
using Cardinal_System_Shared.Dtos;
using Cardinal_System_Shared.Dtos.Entity;

namespace Cardinal_System_Shared.Entity
{
    public class PhysicalEntity : Entity
    {
        private Tuple<float, float> _xyPos;

        public Tuple<float, float> Position
        {
            get { return _xyPos; }
        }

        public PhysicalEntity()
        {
            _xyPos = new Tuple<float, float>(0, 0);
        }

        public PhysicalEntity(long id, Tuple<float, float> initialTuple)
            : base(true)
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
                        _xyPos = new Tuple<float, float>(physicalMovementEntityChange.NewPosition.Item1 + _xyPos.Item1,
                            physicalMovementEntityChange.NewPosition.Item2 + _xyPos.Item2);
                    break;
            }
        }

        public override EntityType GetEntityType()
        {
            return EntityType.BasicPhysicalEntity;
        }
    }
}
